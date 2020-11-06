using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Ball : MonoBehaviour
{
    public Vector3 impulse;
    bool shootButtonWasPressed = false;
    int amountOfJumpsPerBall = 1;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Ball instantiated");
    }


    // Update is called once per frame    
    
    void Update()
    {
    
    }
    private void OnCollisionEnter(Collision collision) //destroys ball after hitting an object (ground)
    {
        if ((collision.gameObject.name == "Plane") || (collision.gameObject.name == "Floor")) //if the name of the ground is changed this has to change as well
        { 
            Debug.Log("destroy");
            Destroy(this.gameObject);
            Destroy(GameObject.Find("BullsEye"));
        }
    }
}