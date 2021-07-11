using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] GameObject hands;
    [SerializeField] GameObject health;

    [SerializeField] bool handsClock;
    [SerializeField] bool explodeClock;
    [SerializeField] bool healthClock;

    private bool destroyed = false;
    private Animator an;

    private void Start()
    {
        an = GetComponentInChildren < Animator >();
    }

    public IEnumerator DestroyClock()
    {
        if (!destroyed)
        {
            destroyed = true;
            FindObjectOfType<MusicPlayer>().Play("ClockBreak1");
            an.SetTrigger("Broken");

            if (handsClock)
            {
                GameObject handOne = Instantiate(hands, transform.position, Quaternion.identity);
                GameObject handTwo = Instantiate(hands, transform.position, Quaternion.identity);
                handOne.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 2);
                handTwo.GetComponent<Rigidbody2D>().velocity = new Vector2(1, 2);
            }
            else if (explodeClock)
            {
                //explode
            }
            else if (healthClock)
            {
                GameObject handOne = Instantiate(health, transform.position, Quaternion.identity);
                GameObject handTwo = Instantiate(health, transform.position, Quaternion.identity);
                handOne.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 2);
                handTwo.GetComponent<Rigidbody2D>().velocity = new Vector2(1, 2);
            }
            

            yield return StartCoroutine(GameObject.Find("GameManager").GetComponent<LevelManager>().StopEnemy());

            Destroy(gameObject);
        }
    }
}
