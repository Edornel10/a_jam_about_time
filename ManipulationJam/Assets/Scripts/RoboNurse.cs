using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RoboNurse : MonoBehaviour
{
    [Header("Idle Move")]
    [SerializeField] float speedIdle;
    [SerializeField] float speedFlee;
    [Space]
    [SerializeField] float stoppingPointLeft;
    [SerializeField] float stoppingPointRight;
    [Space]
    [SerializeField] float timeOfFleeing;

    [Header("Pathfinding")]
    [SerializeField] float activateDistance = 50f;
    [SerializeField] float pathUpdateSeconds = 0.5f;

    [Header("Physicis")]
    [SerializeField] LayerMask lmWalls;
    [SerializeField] float speed = 200f;
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
    bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D rb;
    BoxCollider2D bc;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, player.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // See if colliding with anything
        bool isGrounded = bc.IsTouchingLayers(lmWalls);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }

        // Movement
        rb.velocity = new Vector2(force.x, rb.velocity.y);

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
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, player.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    IEnumerator MoveIdle()
    {
        yield return null;
    }

    IEnumerator Flee()
    {
        yield return null;
    }

}
