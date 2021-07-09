using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] GameObject Hands;

    private bool destroyed = false;

    public IEnumerator DestroyClock()
    {
        if (!destroyed)
        {
            destroyed = true;
            yield return StartCoroutine(GameObject.Find("GameManager").GetComponent<LevelManager>().StopEnemy());

            Instantiate(Hands, transform.position, Quaternion.identity);
            Instantiate(Hands, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
