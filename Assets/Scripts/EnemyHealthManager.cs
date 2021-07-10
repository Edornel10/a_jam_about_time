using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] private float health = 100.0f;
    [SerializeField] private GameObject deathExplosion;
    [SerializeField] private string damageSound; 

    private float shakeMagnitude = 1;
    private float shakeDuration = .2f;
    private float frequency = .3f;

    private CinemachineShake cs;
    private HealthBar healthBar;

    bool shaken = false;

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetMaxLife(maxHealth);
        health = maxHealth;
        cs = GameObject.Find("CM vcam1").GetComponent<CinemachineShake>();
    }

    private void Update()
    {
        
        if (health <= 0 && !shaken)
        {
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
            cs.Shake(shakeDuration, shakeMagnitude, frequency);
            Destroy(gameObject);
        }
        
    }

    public void InflictDamage(float d)
    {
        FindObjectOfType<MusicPlayer>().Play(damageSound);
        health -= d;
        healthBar.SetLife(health);
    }
}
