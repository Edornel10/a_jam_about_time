using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToLive = 5f;
    [SerializeField] float damage = 15f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyObject", timeToLive);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            PlayerHealthManager hm;
            if (collision.transform.TryGetComponent<PlayerHealthManager>(out hm))
            {
                hm.InflictDamage(damage);
                //SendMessageUpwards("OnAttack", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
