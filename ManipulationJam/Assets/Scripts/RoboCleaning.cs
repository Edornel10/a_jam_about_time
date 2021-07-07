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
    [SerializeField] float waitAfterAttack;
    [SerializeField] float attackRange;
    [SerializeField] float attackSpeed;
    [SerializeField] float attackTime;
    [SerializeField] float damage;
    [SerializeField] Transform attackPointA;
    [SerializeField] Transform attackPointB;

    Rigidbody2D rb;
    Transform player;
    BoxCollider2D bc;
    Animator an;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bc = GetComponent<BoxCollider2D>();
        an = GetComponentInChildren<Animator>();

        an.SetBool("Walk", true);
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

            rb.velocity = new Vector2(speedIdle * Time.fixedDeltaTime, rb.velocity.y);

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
        an.SetBool("PreAttack", true);
        Vector2 direction = player.position - transform.position;
        rb.velocity = Vector2.zero;

        if ((player.position - transform.position).x > 0)
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if ((player.position - transform.position).x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        yield return new WaitForSeconds(waitUntilAttack);
        an.SetBool("PreAttack", false);
        an.SetBool("Attack", true);
        //bc.isTrigger = true;

        if (direction.x < 0)
            attackSpeed = -Mathf.Abs(attackSpeed);
        else if (direction.x > 0)
            attackSpeed = Mathf.Abs(attackSpeed);

        rb.velocity = new Vector2(attackSpeed, rb.velocity.y);

        //DoDamage
        float time = attackTime;
        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
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
        rb.velocity = Vector2.zero;
        an.SetBool("Attack", false);
        an.SetBool("Walk", false);
        yield return new WaitForSeconds(waitAfterAttack);
        an.SetBool("Walk", true);
        //bc.isTrigger = false;
    }
}
