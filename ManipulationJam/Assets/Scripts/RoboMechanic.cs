using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboMechanic : MonoBehaviour
{
    [Header("Idle Move")]
    [SerializeField] float speedIdle;
    [SerializeField] float stoppingPointLeft;
    [SerializeField] float stoppingPointRight;

    [Header("Attack")]
    [SerializeField] float waitUntilAttack;
    [SerializeField] float waitAfterAttack;
    [SerializeField] float attackRange;
    [SerializeField] float attackTime;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;
    [SerializeField] float spread;
    [SerializeField] GameObject bullet;

    Rigidbody2D rb;
    BoxCollider2D bc;
    Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bc = GetComponent<BoxCollider2D>();

        StartCoroutine(IdleMovement());
    }

    IEnumerator IdleMovement()
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, player.position) < attackRange)
            {
                yield return StartCoroutine(Attack());
            }

            if (transform.position.x < stoppingPointLeft)
                speedIdle = Mathf.Abs(speedIdle);
            else if (transform.position.x > stoppingPointRight)
                speedIdle = -Mathf.Abs(speedIdle);

            rb.velocity = new Vector2(speedIdle * Time.deltaTime, rb.velocity.y);

            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            yield return null;
        }

    }

    IEnumerator Attack()
    {
        Vector2 direction = player.position - transform.position;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(waitUntilAttack);

        GameObject shot = Instantiate(bullet, transform.position, Quaternion.identity);
        shot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(direction.x) * bulletSpeed, spread);

        shot = Instantiate(bullet, transform.position, Quaternion.identity);
        shot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(direction.x) * bulletSpeed, 0);

        shot = Instantiate(bullet, transform.position, Quaternion.identity);
        shot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(direction.x) * bulletSpeed, -spread);

        yield return new WaitForSeconds(waitAfterAttack);

    }
}
