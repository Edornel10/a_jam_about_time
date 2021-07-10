using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] float enemyStopTime;

    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] Transform[] Enemies;
    [SerializeField] Transform[] Doors;

    private void Start()
    {
        StartCoroutine(UpdateDoorsEnemies());
    }

    IEnumerator UpdateDoorsEnemies()
    {
        Enemies[0].gameObject.SetActive(true);
        for (int i = 0; i < Enemies.Length -1; i++)
        {
            yield return new WaitUntil(() => Enemies[i].childCount <= 0);
            Doors[i].gameObject.SetActive(false);
            Enemies[i + 1].gameObject.SetActive(true);
        }
        yield return null;
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
