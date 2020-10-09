using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 impulse = new Vector3(0.0f, 0.0f, 0.0f);
    bool shootButtonWasPressed = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shootButtonWasPressed = true;
        }
    }

    void FixedUpdate()
    {
        if (shootButtonWasPressed)
        {
            GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
            shootButtonWasPressed = false;
        }
    }
}