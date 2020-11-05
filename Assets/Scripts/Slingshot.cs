using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Slingshot : MonoBehaviourPun
{
    float compass_input;

    // Start is called before the first frame update
    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Ignore everything if this is another player's object
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        compass_input = Input.compass.magneticHeading;
    }

    void FixedUpdate()
    {
        // Ignore everything if this is another player's object
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, compass_input, 0), Time.deltaTime * 3);
    }
}
