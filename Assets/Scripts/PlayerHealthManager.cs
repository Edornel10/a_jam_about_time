using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] float invisibleTimeAfterDamage = 0.3f;

    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] private float health;

    private float shakeMagnitude = 1;
    private float shakeDuration = .2f;
    private float frequency = .3f;


    private CinemachineShake cs;
    private HealthBar healthBar;
    private SpriteRenderer sp;
    private Animator an;

    bool shaken = false;
    float damageTimer;

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetMaxLife(maxHealth);
        health = maxHealth;
        cs = GameObject.Find("CM vcam1").GetComponent<CinemachineShake>();
        sp = GetComponentInChildren<SpriteRenderer>();
        an = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (health <= 0 && !shaken)
        {
            //Do something
        }
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
        else
        {
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1f);
        }

    }

    public void InflictDamage(float d)
    {
        if (damageTimer <= 0)
        {
            an.SetTrigger("Damage");
            damageTimer = invisibleTimeAfterDamage;
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, .5f);
            health -= d;
            healthBar.SetLife(health);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Health")
        {
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, .5f);
            health += 20;
            if (health > maxHealth)
                health = maxHealth;
            healthBar.SetLife(health);
            Destroy(collision.gameObject);
        }
    }
}
