using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform attackPointA;
    [SerializeField] Transform attackPointB;
    [SerializeField] float damage = 20f;
    [SerializeField] float attackTime = 0.5f;

    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        float time = attackTime;
        bool alreadyAttacked = false; ;
        while (time > 0)
        {
            time -= Time.deltaTime;
            if (!alreadyAttacked)
            {
                Collider2D[] colliderList = Physics2D.OverlapAreaAll(attackPointA.position, attackPointB.position);
                foreach (Collider2D collider in colliderList)
                {
                    //                print(collider.transform.tag);
                    if (collider.transform.tag == "Enemy")
                    {
                        EnemyHealthManager hm;
                        if (collider.transform.TryGetComponent<EnemyHealthManager>(out hm))
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
    }
}
