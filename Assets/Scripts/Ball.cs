using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Ball : MonoBehaviourPun
{
    public Vector3 impulse;
    bool shootButtonWasPressed = false;
    int amountOfJumpsPerBall = 1;
    public GameObject creator;

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
        GameObject col = collision.gameObject;
        if (col.name == "Plane") //if the name of the ground is changed this has to change as well
        { 
            Debug.Log("destroy");
            //PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
            Destroy(this.gameObject);
        }

        if (col.CompareTag("Player") && !GameObject.ReferenceEquals(col, this.transform.root))
        {
            Debug.Log("Player has been hit");
            //Destroy(this.gameObject);
        }
    }
}