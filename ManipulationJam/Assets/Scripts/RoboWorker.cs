using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboWorker : MonoBehaviour
{
    [Header("Idle Move")]
    [SerializeField] float speedIdle;
    [SerializeField] float stoppingPointLeft;
    [SerializeField] float stoppingPointRight;

    [Header("Attack")]
    [SerializeField] float waitUntilAttack;
    [SerializeField] float attackAnimTime;
    [SerializeField] float waitAfterAttack;
    [SerializeField] float attackRange;
    [SerializeField] float attackTime;
    [SerializeField] float damage;
    [SerializeField] Transform attackPointA;
    [SerializeField] Transform attackPointB;

    Rigidbody2D rb;
    BoxCollider2D bc;
    Transform player;
    Animator an;
    LevelManager lm;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bc = GetComponent<BoxCollider2D>();
        an = GetComponentInChildren<Animator>();
        lm = GameObject.Find("GameManager").GetComponent<LevelManager>();

        an.SetBool("Walk", true);

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
        FindObjectOfType<MusicPlayer>().Play("Ow");
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
        an.SetBool("PreAttack", true);

        yield return new WaitForSeconds(waitUntilAttack);

        an.SetBool("Attack", true);
        an.SetBool("PreAttack", false);

        yield return new WaitForSeconds(attackAnimTime);


        //DoDamage
        float time = attackTime;
        bool alreadyAttacked = false; ;
        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            if(!alreadyAttacked)
            {
                Collider2D[] colliderList = Physics2D.OverlapAreaAll(attackPointA.position, attackPointB.position);
                foreach (Collider2D collider in colliderList)
                {
    //                print(collider.transform.tag);
                    if (collider.transform.tag == "Player")
                    {
                        PlayerHealthManager hm;
                        if (collider.transform.TryGetComponent<PlayerHealthManager>(out hm))
                        {
                            hm.InflictDamage(damage);
                            alreadyAttacked = true;
                            //SendMessageUpwards("OnAttack", SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(waitAfterAttack);
        an.SetBool("Attack", false);

    }

}
