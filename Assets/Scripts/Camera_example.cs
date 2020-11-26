using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Camera_example : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected && !this.transform.parent.GetComponent<PhotonView>().IsMine)
        {
            gameObject.GetComponent<Camera>().enabled = false;
            gameObject.GetComponent<AudioListener>().enabled = false;
        }
    }
}
