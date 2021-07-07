using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] float enemyStopTime;

    public bool stopEnemy = false;
    [SerializeField] Rigidbody2D playerRB;

    private void Update()
    {
        if (stopEnemy)
        {
            stopEnemy = false;
            StartCoroutine(StopEnemy());
        }
    }

    public IEnumerator StopEnemy()
    {
        playerRB.velocity /= 0.2f;
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        yield return new WaitForSeconds(enemyStopTime);
        playerRB.velocity *= 0.2f;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02F;
        yield return null;
    }

    public void LoadLevel(string name)
    {
        Debug.Log("Ich lade Scene: " + name);
        SceneManager.LoadScene(name);
    }

    public void QuitLevel()
    {
        Debug.Log("Das Game wurde beendet");
        Application.Quit();
    }
}
