// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/rustShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _RustColorRed("RustColorRed",Color) = (.7176,.2549,.0549,1)
        _RustColorGreen("RustColorGreen",Color) =(0.5882,0.8235,0.7176,1)
        _OutlineColor("Outline", Color) = (1,1,1,1)
        _Rust("Rust",Float) = 0
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            Pass
            {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ PIXELSNAP_ON
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    half2 texcoord  : TEXCOORD0;
                };

                fixed4 _Color;
                fixed4 _OutlineColor;
                fixed4 _RustColorRed;
                fixed4 _RustColorGreen;

                float _Rust;
                float _TexWidth;
                float _TexHeight;

                v2f vert(appdata_t IN)
                {
                    v2f OUT;
                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord = IN.texcoord;
                    OUT.color = IN.color * _Color;
                    #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
                    #endif

                    return OUT;
                }

                sampler2D _MainTex;
                float4 _MainTex_TexelSize;


                fixed4 frag(v2f IN) : SV_Target
                {
                    fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                    if (c.a == 0) {
                        return fixed4(0, 0, 0, 0); // Skip outline for transparent pixels
                    }

                    // Get the colors of the surrounding pixels
                    fixed4 up = tex2D(_MainTex, IN.texcoord + fixed2(0, _MainTex_TexelSize.y));
                    fixed4 down = tex2D(_MainTex, IN.texcoord - fixed2(0, _MainTex_TexelSize.y));
                    fixed4 left = tex2D(_MainTex, IN.texcoord - fixed2(_MainTex_TexelSize.x, 0));
                    fixed4 right = tex2D(_MainTex, IN.texcoord + fixed2(_MainTex_TexelSize.x, 0));

                    float rand = 0.00;
                    {
                        float g = 12.9898;
                        float d = 78.233;
                        float e = 43758.5453;
                        float dt = dot(IN.texcoord, float2(g, d));
                        float sn = fmod(dt, 3.14);
                        rand = frac(sin(sn) * e);
                    }

                    float param = abs(IN.texcoord.x - 0.5) + abs(IN.texcoord.y - 0.5);

                    // This method uses an if statement
                    
                    

                    
                    if ( (rand+param) * _Rust >1.00 && ((c.r + c.g + c.b) > .1)) {
                        if(c.r>(c.g+c.b)){
                            c.rgb *= _RustColorGreen.rgb;
                            //c.rgb *= _RustColorGreen.rgb;
                        }
                        else{
                            c.rgb *= _RustColorRed.rgb;
                            //c.rgb *= _RustColorRed.rgb;
                        }
                    }

                    c.rgb *= c.a;
                    return c;
                }
            ENDCG
            }
        }
}