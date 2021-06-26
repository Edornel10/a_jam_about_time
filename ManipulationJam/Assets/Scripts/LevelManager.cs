using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
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
