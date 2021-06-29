using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboCleaning : MonoBehaviour
{
    [Header("Idle Move")]
    [SerializeField] float speedIdle;
    [SerializeField] float stoppingPointLeft;
    [SerializeField] float stoppingPointRight;

    [Header("Attack")]
    [SerializeField] float waitUntilAttack;
    [SerializeField] float attackRange;
    [SerializeField] float attackSpeed;
    [SerializeField] float attackTime;
    [SerializeField] float damage;
    [SerializeField] Transform attackPointA;
    [SerializeField] Transform attackPointB;

    Rigidbody2D rb;
    Transform player;
    BoxCollider2D bc;

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
            if(Vector2.Distance(transform.position, player.position) < attackRange)
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

        bc.isTrigger = true;

        if (direction.x < 0)
            attackSpeed = -Mathf.Abs(attackSpeed);
        else if (direction.x > 0)
            attackSpeed = Mathf.Abs(attackSpeed);

        rb.velocity = new Vector2(attackSpeed, rb.velocity.y);

        //DoDamage
        float time = attackTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            Collider2D[] colliderList = Physics2D.OverlapAreaAll(attackPointA.position, attackPointB.position);
            foreach (Collider2D collider in colliderList)
            {
                print(collider.transform.tag);
                if (collider.transform.tag == "Player")
                {
                    PlayerHealthManager hm;
                    if (collider.transform.TryGetComponent<PlayerHealthManager>(out hm))
                    {
                        hm.InflictDamage(damage);
                        //SendMessageUpwards("OnAttack", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            yield return null;
        }
        bc.isTrigger = false;
    }
}
