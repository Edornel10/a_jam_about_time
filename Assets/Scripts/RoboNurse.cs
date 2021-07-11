using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Pathfinding;

public class RoboNurse : MonoBehaviour
{
    [Header("Idle Move")]
    [SerializeField] float speedIdle = 150f;
    [SerializeField] float speedFlee = 300f;
    [Space]
    [SerializeField] float stoppingPointLeft;
    [SerializeField] float stoppingPointRight;
    [Space]
    [SerializeField] float distanceToFlee = 2f;

    [Header("Pathfinding")]
    [SerializeField] float activateDistance = 50f;
    [SerializeField] float timeOfFleeing = 4f;
    [SerializeField] float pathUpdateSeconds = 0.5f;
    [SerializeField] Transform[] targets;

    [Header("Physicis")]
    [SerializeField] LayerMask lmWalls;
    [SerializeField] float maxVelocity = 1f;
    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float jumpNodeHeightRequirement = 0.8f;
    [SerializeField] float jumpModifier = 0.3f;
    [SerializeField] float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    [SerializeField] bool followEnabled = true;
    [SerializeField] bool jumpEnabled = true;
    [SerializeField] bool directionLookEnabled = true;


    Transform player;
    //private Path path;
    private int currentWaypoint = 0;
    //private int currentPath = 0;
    private float fleeTimer = 0f;
    private Vector2 startPos;
    bool isGrounded = false;
    bool goBack = false;
    //Seeker seeker;
    Rigidbody2D rb;
    CircleCollider2D cc;
    Animator an;
    LevelManager lm;


    void Start()
    {
        startPos = transform.position;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        //seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        lm = GameObject.Find("GameManager").GetComponent<LevelManager>();
        an = GetComponentInChildren<Animator>();

        StartCoroutine(Movement());
        //StartCoroutine(InvokeRepeat());
    }

    private IEnumerator Movement()
    {
        while (true)
        {

            float distance = Vector2.Distance(rb.position, player.transform.position);
            if (distance < distanceToFlee || fleeTimer > 0)
            {
                FindObjectOfType<MusicPlayer>().Stop("NurseMove");

                if (distance < distanceToFlee)
                    fleeTimer = timeOfFleeing;
                StartCoroutine(PathFollow());
                followEnabled = true;
                an.SetBool("Run", true);
            }
            else
            {
                FindObjectOfType<MusicPlayer>().Play("NurseMove");

                followEnabled = false;
                an.SetBool("Run", false);

                if (transform.position.x < stoppingPointLeft)
                    speedIdle = Mathf.Abs(speedIdle);
                else if (transform.position.x > stoppingPointRight)
                    speedIdle = -Mathf.Abs(speedIdle);
                rb.velocity = new Vector2(speedIdle * Time.fixedDeltaTime, rb.velocity.y);

                if (directionLookEnabled)
                {
                    if (rb.velocity.x > Mathf.Epsilon)
                    {
                        transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    else if (rb.velocity.x < -Mathf.Epsilon)
                    {
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                }

                if(transform.position.x - 1 < stoppingPointLeft || transform.position.x + 1 > stoppingPointRight)
                {
                    //seeker.StartPath(rb.position, startPos, OnPathComplete);

                    // Set current waypoint.
                    goBack = true;
                    yield return StartCoroutine(GoToStartPos());
                    goBack = false;
                }
            }
            fleeTimer -= Time.fixedDeltaTime;
            yield return null;
        }
    }

    private IEnumerator GoToStartPos()
    {
        float distance = Vector2.Distance(rb.position, player.transform.position);
        while (distance < nextWaypointDistance)
        {
            // Direction Calculation
            Vector2 direction = (startPos - rb.position).normalized;

            // Movement
            if (direction.x > 0)
                rb.velocity = new Vector2(speedFlee * Time.fixedDeltaTime, rb.velocity.y);
            else
                rb.velocity = new Vector2(-speedFlee * Time.fixedDeltaTime, rb.velocity.y);
            yield return null;
        }
        yield return null;
    }


    private IEnumerator PathFollow()
    {


        if (currentWaypoint >= targets.Length && currentWaypoint > 0)
        {
            currentWaypoint = 0;
        }

        // See if colliding with anything
        bool isGrounded = cc.IsTouchingLayers(lmWalls);

        // Direction Calculation
        Vector2 direction = ((Vector2)targets[currentWaypoint].position - rb.position).normalized;


        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                yield return StartCoroutine(Jump());               
            }
        }

        // Movement
        if(direction.x > 0)
            rb.velocity = new Vector2(speedFlee * Time.fixedDeltaTime, rb.velocity.y);
        else
            rb.velocity = new Vector2(-speedFlee * Time.fixedDeltaTime, rb.velocity.y);


        // Next Waypoint
        float distance = Vector2.Distance(rb.position, (Vector2)targets[currentWaypoint].position);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > Mathf.Epsilon)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -Mathf.Epsilon)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        yield return null;
    }

    private IEnumerator Jump()
    {
        //yield return UpdatePath();
        Vector2 direction = ((Vector2)targets[currentWaypoint].position - rb.position).normalized;
        if (direction.y > jumpNodeHeightRequirement)
        {
            rb.velocity = new Vector2(rb.velocity.x, direction.y * jumpModifier);
        }
        yield return null;
    }
}
