using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sceneswitcher : MonoBehaviour
{
    public void GotoSampleScene()
    {
        SceneManager.LoadScene("SampleScene 1");
    }
    public void GotoImageRecognitionScene()
    {
        SceneManager.LoadScene("ImageRecognitionScene", LoadSceneMode.Additive);
    }
    public void GotoMPTestRoom()
    {
        //SceneManager.LoadScene("MPTestRoom");
        SceneManager.UnloadScene("ImageRecognitionScene");
    }
}
