using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadImageRecognition : MonoBehaviour
{
    private PlayerManager playermanager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("dead") == 1/*playermanager.dead == true*/)
        {
            //Debug.Log("Dead = true");
            SceneManager.UnloadScene("ImageRecognitionScene");
        }
    }
}
