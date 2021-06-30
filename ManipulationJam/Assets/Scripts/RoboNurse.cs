using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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
    private Path path;
    private int currentWaypoint = 0;
    private int currentPath = 0;
    private float fleeTimer = 0f;
    private Vector2 startPos;
    bool isGrounded = false;
    bool goBack = false;
    Seeker seeker;
    Rigidbody2D rb;
    CircleCollider2D cc;

    void Start()
    {
        startPos = transform.position;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();

        StartCoroutine(Movement());
        StartCoroutine(InvokeRepeat());
    }

    private IEnumerator Movement()
    {
        while (true)
        {

            float distance = Vector2.Distance(rb.position, player.transform.position);
            if (distance < distanceToFlee || fleeTimer > 0)
            {
                if (distance < distanceToFlee)
                    fleeTimer = timeOfFleeing;
                StartCoroutine(PathFollow());
                followEnabled = true;
            }
            else
            {
                followEnabled = false;


                if (transform.position.x < stoppingPointLeft)
                    speedIdle = Mathf.Abs(speedIdle);
                else if (transform.position.x > stoppingPointRight)
                    speedIdle = -Mathf.Abs(speedIdle);

                rb.velocity = new Vector2(speedIdle * Time.deltaTime, rb.velocity.y);
                if(transform.position.x - 1 < stoppingPointLeft || transform.position.x + 1 > stoppingPointRight)
                {
                    seeker.StartPath(rb.position, startPos, OnPathComplete);
                    goBack = true;
                    yield return StartCoroutine(GoToStartPos());
                    goBack = false;
                }
            }
            fleeTimer -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator GoToStartPos()
    {
        while(transform.position.x < stoppingPointLeft || transform.position.x > stoppingPointRight)
        {
            yield return StartCoroutine(PathFollow());
            yield return null;
        }
        yield return null;
    }

    private IEnumerator InvokeRepeat()
    {
        while (true)
        {
            StartCoroutine(UpdatePath());
            yield return new WaitForSeconds(pathUpdateSeconds);
        }
    }

    private IEnumerator UpdatePath()
    {

        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, targets[currentPath].position, OnPathComplete);
        }
        if(goBack)
        {
            seeker.StartPath(rb.position, startPos, OnPathComplete);
        }
        yield return null;
    }

    private IEnumerator PathFollow()
    {
        if (path == null)
        {
            yield return null;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            currentWaypoint = 0;
            currentPath++;
            if (currentPath >= targets.Length)
                currentPath = 0;
            UpdatePath();
        }

        // See if colliding with anything
        bool isGrounded = cc.IsTouchingLayers(lmWalls);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        //Vector2 force = direction * speed * Time.deltaTime;

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
            rb.velocity = new Vector2(speedFlee * Time.deltaTime, rb.velocity.y);
        else
            rb.velocity = new Vector2(-speedFlee * Time.deltaTime, rb.velocity.y);


        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        yield return null;
    }

    private IEnumerator Jump()
    {
        yield return UpdatePath();
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        if (direction.y > jumpNodeHeightRequirement)
        {
            rb.velocity = new Vector2(rb.velocity.x, direction.y * jumpModifier);
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, targets[currentPath].transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
        }
    }
}
