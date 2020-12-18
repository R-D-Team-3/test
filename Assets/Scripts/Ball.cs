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
            photonView.RPC("Explode", RpcTarget.All);
            ExplosionDamage(transform.position, 2f);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PhotonView>().ViewID == this.FiringPlayer_ID)
            {
                Debug.Log("Player self hit");
            }
            else
            {
                photonView.RPC("Explode", RpcTarget.All);
                FiringPLayer = PhotonView.Find(FiringPlayer_ID).gameObject.GetComponent<PlayerManager>();
                FiringPLayer.Hits(30,collision.gameObject.GetComponent<PlayerManager>().playerIsTeamBlue);
                collision.gameObject.GetComponent<PlayerManager>().getHit(30,FiringPLayer.playerIsTeamBlue);
                Debug.Log("Player "+FiringPlayer_ID+" hit"+collision.gameObject.GetComponent<PhotonView>().ViewID+"!");
                FiringPLayer.playSound();
            }
        }
    }
    [PunRPC]
    void Explode()
    {
        GameObject o = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(o, 1);
        PhotonNetwork.Destroy(this.gameObject);

    }
    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.name == "Player(Clone)")
            {
                FiringPLayer = PhotonView.Find(FiringPlayer_ID).gameObject.GetComponent<PlayerManager>();
                float distance = 1 / Vector3.Distance(center, hitCollider.transform.position);
                FiringPLayer.Hits(Mathf.FloorToInt(distance * 20), hitCollider.gameObject.GetComponent<PlayerManager>().playerIsTeamBlue);
                hitCollider.gameObject.GetComponent<PlayerManager>().getHit(Mathf.FloorToInt(distance * 20), FiringPLayer.playerIsTeamBlue);
                Debug.Log(FiringPlayer_ID + " " + hitCollider.gameObject.GetComponent<PhotonView>().ViewID);
            }
        }
    }
}