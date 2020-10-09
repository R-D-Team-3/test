using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    bool reload = false;
    int amountOfBalls = 0;

    // Reference to the Prefab of a ball. Drag a Prefab into this field in the Inspector.
    public GameObject ballPrefab;
    public GameObject ballPrefabNew;

    // Start is called before the first frame update
    void Start()
    {
        ballPrefabNew = Instantiate(ballPrefab, new Vector3(0, 5, -5), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && amountOfBalls == 1)
        {
            amountOfBalls--;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && amountOfBalls == 0 && ballPrefabNew == null)
        {
            reload = true;
        }
    }

    void FixedUpdate()
    {
        if (reload)
        {
            ballPrefabNew = Instantiate(ballPrefab, new Vector3(0, 5, -5), Quaternion.identity);
            reload = false;
            amountOfBalls++;
            Debug.Log("reloaded, amount left:" + amountOfBalls);
        }

    }
}
