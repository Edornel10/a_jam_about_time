Shader "Unlit/explodeShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _OutlineColor("Outline", Color) = (1,1,1,1)
        _Explode("Explode",Float) = 0
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
                float _Explode;
                float _TexWidth;
                float _TexHeight;

                v2f vert(appdata_t IN)
                {
                    v2f OUT;
                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord = IN.texcoord;
                    OUT.color = IN.color * _Color;
                    #ifdef PIXELSNAP_OFF
                    CULL_OFF
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
                    #endif

                    return OUT;
                }

                sampler2D _MainTex;
                float4 _MainTex_TexelSize;


                fixed4 frag(v2f IN) : SV_Target
                {
                    //fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                    fixed4 shift;
                    
                    fixed2 param;
                    param.x = IN.texcoord.x - 0.5;
                    param.y = IN.texcoord.y - 0.5;

                    //shift = tex2D(_MainTex, IN.texcoord + fixed2(_MainTex_TexelSize.x * paramX * _Explode / 100.00, _MainTex_TexelSize.y * paramY *_Explode/100.00));

                    param.x *= _Explode / 50.00;
                    param.y *= _Explode / 50.00;

                    fixed4 c = tex2D(_MainTex, IN.texcoord-param);
                    //c.a = 1;//= tex2D(_MainTex, IN.texcoord - param).a;
                    //c.a = 

                    //reset param values
                    param.x = IN.texcoord.x - 0.5;
                    param.y = IN.texcoord.y - 0.5;

                    float Pwidth = _MainTex_TexelSize.z; // texel width
                    float Pheight = _MainTex_TexelSize.w; // texel height
                    
                    if ((abs(param.x * Pwidth) % (.001 + (Pwidth * _Explode/60))) < (_Explode * Pwidth/120)) {
                        c.a *= 0;
                        c.r = 0;
                        c.g = 0;
                        c.b = 0;
                    }

                    if ((abs(param.y * Pheight) % (.001 + (Pheight * _Explode/60))) < (_Explode * Pheight / 120)) {
                        c.a *= 0;
                        c.r = 0;
                        c.g = 0;
                        c.b = 0;
                    }
                    

                    if (c.r == 0 && c.b == 0 && c.g == 0)
                        c.a = 0;

                    c.a *= (50.00-_Explode) /50.00;
                    c.r *= (50.00 - _Explode) / 50.00;
                    c.g *= (50.00 - _Explode) / 50.00;
                    c.b *= (50.00 - _Explode) / 50.00;



                    return c;
                }
            ENDCG
            }
        }
}
