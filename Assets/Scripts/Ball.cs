using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 impulse = new Vector3(0.0f, 0.0f, 0.0f);
    bool shootButtonWasPressed = false;
    bool reload = false;
    int amountOfBalls = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && amountOfBalls > 0)
        {
            shootButtonWasPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.R) && amountOfBalls == 0)
        {
            reload = true;
        }
    }

    void FixedUpdate()
    {
        if (shootButtonWasPressed)
        {
            GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
            shootButtonWasPressed = false;
            amountOfBalls--;
        }

        if (reload)
        {
            reload = false;
            amountOfBalls++;
        }
    }
}