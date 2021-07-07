using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
   
    Rigidbody2D myRigidbody;
    //Animator myAnimator;
    SpriteRenderer mySpriteRenderer;
    BoxCollider2D myBoxCollider;
    [SerializeField] ParticleSystem moveParticles;
    

    [SerializeField] LayerMask lmWalls;
    [SerializeField] LayerMask lmLadder;
    [SerializeField] float fJumpVelocity = 5;
    [SerializeField] float fJumpPressedRememberTime = 0.2f;  
    [SerializeField] float fGroundedRememberTime = 0.25f;
    [SerializeField] float fHorizontalAcceleration = 1;
    [SerializeField] float fHorizontalAirAcceleration = .5f;
    [SerializeField] float fHorizontalBaseSpeed = 10;
    [SerializeField] [Range(0, 1)] float fHorizontalDampingBasic = 0.5f;
    [SerializeField] [Range(0, 1)] float fHorizontalDampingWhenStopping = 0.5f;
    [SerializeField] [Range(0, 1)] float fHorizontalDampingWhenTurning = 0.5f;
    [SerializeField] [Range(0, 1)] float fCutJumpHeight = 0.5f;
    [SerializeField] float dashTime = 0.5f;
    [SerializeField] float rollSpeed = 1f;
    [SerializeField] float climbSpeed;

    private float fHorizontalSpeed = 10;
    private float fGroundedRemember = 0;
    private float fJumpPressedRemember = 0;
    private float fDash = 0;
    private float fHorizontalVelocity;
    private bool fFacingRight;
    private float gravityScale;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        //myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        myBoxCollider = GetComponent<BoxCollider2D>();

        gravityScale = myRigidbody.gravityScale;
    }

    void Update()
    {
        //Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
        //Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
        //bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);
        bool bGrounded = myBoxCollider.IsTouchingLayers(lmWalls);

        Jump(bGrounded);
        Dash(bGrounded);
    }

    private void LateUpdate()
    {
        bool bGrounded = myBoxCollider.IsTouchingLayers(lmWalls);
        bool climbing = myBoxCollider.IsTouchingLayers(lmLadder);
        Run(bGrounded, climbing);
    }

    private void Run(bool bGrounded, bool climbing)
    {
        
        // Move
        fHorizontalVelocity = myRigidbody.velocity.x;
        if (bGrounded)
        {
            fHorizontalSpeed = fHorizontalBaseSpeed * fHorizontalAcceleration / Time.timeScale;
            fHorizontalVelocity += (Input.GetAxisRaw("Horizontal") * fHorizontalAcceleration) / Time.timeScale;
        }
        else
        {
            fHorizontalSpeed = fHorizontalBaseSpeed * fHorizontalAirAcceleration / Time.timeScale;
            fHorizontalVelocity += Input.GetAxisRaw("Horizontal") * fHorizontalAirAcceleration / Time.timeScale;
        }

        // FacingRight?
        if (Mathf.Abs(fHorizontalVelocity) > 0.1f)
            fFacingRight = (Mathf.Sign(Input.GetAxisRaw("Horizontal")) == 1) ? false : true;

        if (fDash > 0)
            if(fFacingRight)
                fHorizontalVelocity = -rollSpeed;
            else
                fHorizontalVelocity = rollSpeed;

        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.fixedDeltaTime * fHorizontalSpeed);
        else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity))
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.fixedDeltaTime * fHorizontalSpeed);
        else
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.fixedDeltaTime * fHorizontalSpeed);

        if(climbing)
        {
            myRigidbody.gravityScale = 0;
            float fVericallVelocity = Input.GetAxis("Vertical") * climbSpeed * Time.fixedDeltaTime/Time.timeScale;
            myRigidbody.velocity = new Vector2(fVericallVelocity, myRigidbody.velocity.y);
            myRigidbody.velocity = new Vector2(fHorizontalVelocity, fVericallVelocity);
        }
        else
        {
            myRigidbody.gravityScale = gravityScale / Time.timeScale / Time.timeScale;
            myRigidbody.velocity = new Vector2(fHorizontalVelocity, myRigidbody.velocity.y);
        }

        // Animation
        if (Mathf.Abs(fHorizontalVelocity) > 0.1f && bGrounded)
        {
            //myAnimator.SetBool("run", true);
            moveParticles.emissionRate = 2;
        }
        else
        {
            //myAnimator.SetBool("run", false);
            moveParticles.emissionRate = 0;
        }
        // Flip Sprite
        if (fFacingRight)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        //mySpriteRenderer.flipX = !fFacingRight;
    }

    private void Jump(bool bGrounded)
    {
        fGroundedRemember -= (Time.deltaTime / Time.timeScale);
        if (bGrounded)
        {
            fGroundedRemember = fGroundedRememberTime;
        }

        fJumpPressedRemember -= (Time.deltaTime / Time.timeScale);
        if (Input.GetButtonDown("Jump"))
        {
            fJumpPressedRemember = fJumpPressedRememberTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            if (myRigidbody.velocity.y > 0)
            {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y * fCutJumpHeight);
            }
        }

        if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, fJumpVelocity / Time.timeScale);
        }

        if (!bGrounded)
        {
            if (myRigidbody.velocity.y > Mathf.Epsilon)
            {
                //myAnimator.SetInteger("air", 1);
            }
            else
            {
                //myAnimator.SetInteger("air", 2);
            }
        }
        else
        {
            //myAnimator.SetInteger("air", 0);
        }
    }

    private void Dash(bool bGrounded)
    {
        fDash -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            //myAnimator.SetTrigger("roll");
            fDash = dashTime;
        }
    }
}
