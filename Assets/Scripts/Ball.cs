using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Ball : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public Vector3 impulse;
    bool shootButtonWasPressed = false;
    int amountOfJumpsPerBall = 1;
    private Healthbar healthbarScript;
    private PlayerManager FiringPLayer;
    public GameObject explosionEffect;
    int FiringPlayer_ID;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Ball instantiated");
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] Data = info.photonView.InstantiationData;
        FiringPlayer_ID = (int)Data[0];
    
        Debug.Log("Ball instantiated(instantiate) with playerID: "+ FiringPlayer_ID);
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
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PhotonView>().ViewID == this.FiringPlayer_ID)
            {
                Debug.Log("Player self hit");
            }
            else
            {
                photonView.RPC("explode", RpcTarget.All);
                healthbarScript = collision.gameObject.GetComponent<PlayerManager>().healthbarScript;
                healthbarScript.TakeDamage(30);
                FiringPLayer = PhotonView.Find(FiringPlayer_ID).gameObject.GetComponent<PlayerManager>();
                FiringPLayer.GivePoints(30);
                Debug.Log("Player "+FiringPlayer_ID+" hit"+collision.gameObject.GetComponent<PhotonView>().ViewID+"!");
            }
        }
    }
    [PunRPC]
    void explode()
    {

        GameObject o = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(o, 3);
        PhotonNetwork.Destroy(this.gameObject);
    }
}