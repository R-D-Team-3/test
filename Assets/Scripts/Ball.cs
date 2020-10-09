using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 impulse = new Vector3(0.0f, 0.0f, 0.0f);
    bool shootButtonWasPressed = false;
    int amountOfJumpsPerBall = 1;

    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && amountOfJumpsPerBall > 0)
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
            amountOfJumpsPerBall--;
            Debug.Log("shot");
        }

    }

    private void OnCollisionEnter(Collision collision) //destroys ball after hitting an object (ground)
    {
        if (collision.gameObject.name == "Plane") //if the name of the ground is changed this has to change as well
        { 
            Debug.Log("destroy");
            Destroy(this.gameObject);
        }
    }
}