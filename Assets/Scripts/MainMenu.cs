using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartSinglePlayer()
    {
        SceneManager.LoadScene(1);
    }

    public void StartMultiPlayer()
    {
        SceneManager.LoadScene(2);
    }
}
