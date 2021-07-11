using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] float enemyStopTime;
    [SerializeField] GameObject Info;

    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] Transform player;
    [SerializeField] Transform[] Enemies;
    [SerializeField] Transform[] Doors;

    [SerializeField] Material mat;

    private Vector2 startPos;

    private void Start()
    {
        startPos = player.position;
        StartCoroutine(UpdateDoorsEnemies());
        StartCoroutine(WaitUntilMove());
    }

    IEnumerator WaitUntilMove()
    {
        yield return new WaitUntil(() => Vector2.Distance(player.position, startPos) > 5);
        Info.SetActive(false);
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
        float timer = enemyStopTime;
        float rust = 0;
        while(timer > 0)
        {
            mat.SetFloat("_Rust", 2f);
            timer -= Time.fixedDeltaTime/Time.timeScale;
            rust += Time.fixedDeltaTime;
            mat.SetFloat("_Rust", rust);
            yield return null;
        }
        playerRB.velocity *= 0.2f;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02F;
        timer = 3f;
        rust = 2;
        while (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            rust -= Time.fixedDeltaTime * 2;
            mat.SetFloat("_Rust", rust);
            yield return null;
        }
        mat.SetFloat("_Rust", 0);
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
