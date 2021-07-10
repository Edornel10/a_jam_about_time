using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    [SerializeField] float damage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            EnemyHealthManager hm;
            if (collision.transform.TryGetComponent<EnemyHealthManager>(out hm))
            {
                hm.InflictDamage(damage);
                //SendMessageUpwards("OnAttack", SendMessageOptions.DontRequireReceiver);
            }
        }

        else if (collision.transform.tag == "Clock")
        {
            StartCoroutine(collision.GetComponent<Clock>().DestroyClock());
        }
    }
}
