using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartSinglePlayer()
    {
        PlayerPrefs.SetInt("max", 1);
        PlayerPrefs.SetInt("min", 1);
        SceneManager.LoadScene(2);
    }

    public void StartMultiPlayer()
    {
        PlayerPrefs.SetInt("max", 4);
        PlayerPrefs.SetInt("min", 2);
        SceneManager.LoadScene(2);
    }
}
