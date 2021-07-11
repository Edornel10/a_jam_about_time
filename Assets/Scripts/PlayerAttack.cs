using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Melee")]
    [SerializeField] Transform attackPointA;
    [SerializeField] Transform attackPointB;
    [SerializeField] float Mdamage = 20f;
    [SerializeField] float attackTime = 0.5f;
    [SerializeField] float attackWait = 0.5f;
    [SerializeField] float waitUntilAttack = 0.15f;

    [Header("Ranged")]
    [SerializeField] GameObject hands;
    [SerializeField] float Rdamage = 20f;
    [SerializeField] float velocity = 20f;

    private int handsCount = 0;
    private float attackTimer = 0;
    private Animator an;

    void Start()
    {
        an = GetComponentInChildren<Animator>();   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && attackTimer < 0)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.Q) && handsCount > 0 && attackTimer < 0)
        {
            handsCount--;
            StartCoroutine(RangeAttack());
        }
        attackTimer -= Time.deltaTime/Time.timeScale;
    }

    IEnumerator Attack()
    {
        attackTimer = attackWait;
        float time = attackTime;
        bool alreadyAttacked = false;
        an.SetTrigger("Attack");
        yield return new WaitForSeconds(waitUntilAttack*Time.timeScale);
        while (time > 0)
        {
            time -= Time.deltaTime;
            if (!alreadyAttacked)
            {
                Collider2D[] colliderList = Physics2D.OverlapAreaAll(attackPointA.position, attackPointB.position);
                foreach (Collider2D collider in colliderList)
                {
                    if (collider.transform.tag == "Enemy")
                    {
                        EnemyHealthManager hm;
                        if (collider.transform.TryGetComponent<EnemyHealthManager>(out hm))
                        {
                            hm.InflictDamage(Mdamage);
                            alreadyAttacked = true;
                            //SendMessageUpwards("OnAttack", SendMessageOptions.DontRequireReceiver);
                        }
                    }

                    else if (collider.transform.tag == "Clock")
                    {
                        StartCoroutine(collider.GetComponent<Clock>().DestroyClock());
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator RangeAttack()
    {
        attackTimer = attackWait;

        Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        GameObject hand = Instantiate(hands, transform.position, Quaternion.AngleAxis(angle, Vector3
            .forward));
        Vector2 shootDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        hand.GetComponent<Rigidbody2D>().velocity = shootDir.normalized * velocity;
        hand.GetComponent<CircleCollider2D>().isTrigger = true;
        hand.tag = "Untagged";
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Hands")
        {
            handsCount++;
            Destroy(collision.gameObject);
        }
    }
}
