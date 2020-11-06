using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

using Photon.Pun;

public class Rubber : MonoBehaviourPun
{
    // Start is called before the first frame update
    float rubber_strain;
    float rubber_force;
    Touch pulltouch;
    Vector2 pull_pos;
    Vector2 start_pos;
    bool ball_present;
    public GameObject ballPrefab;
    public GameObject holder;
    public GameObject bullseyePrefab;
    public GameObject slingshot;
    GameObject bullseye;
    GameObject throw_ball;
    Vector3 impulse;
    public float angle;
    float dist_slingshot;   // Indicates the distance of the bullseye from the slingshot.
    float airtime;          // The duration between the departure and collision of the ball
    float gravity = (float) 9.81;


    void Start()
    { 
        
        slingshot = this.transform.parent.gameObject;
        ball_present = false;
        rubber_strain = 0f;
        rubber_force = 1f;

        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Ignore everything if this is another player's object
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        angle = Input.acceleration.y * -10;

        if (Input.touchCount > 0 )
        {
            pulltouch = Input.GetTouch(0); 
            start_pos = pulltouch.rawPosition;
            if((start_pos.x > Screen.width/3) && (start_pos.x < 2*Screen.width/3) && (start_pos.y < Screen.height/4 ))
            {
                pull_pos = pulltouch.position;
                if(pull_pos.y < start_pos.y)
                {
                    rubber_strain = (start_pos.y - pull_pos.y)*400/Screen.height;
                    rubber_force = rubber_strain/10;
                    ball_present = true;

                    //Here the position of the bullseye is calculated & adjusted. https://answers.unity.com/questions/1552089/impulse-force-time.html
                    //float upvelocity = angle * 2 / throw_ball.GetComponent<Rigidbody>().mass;
                    //airtime = (upvelocity + Mathf.Sqrt((upvelocity*upvelocity) + (4 * gravity * holder.transform.position.y))) / (2 * gravity);

                    //float forwardvelocity = rubber_strain / (8 * throw_ball.GetComponent<Rigidbody>().mass);
                    //dist_slingshot = airtime * forwardvelocity;
                    //Complete updating position!
                    //bullseye.transform.position = new Vector3(dist_slingshot * (Mathf.Cos(slingshot.transform.rotation.y*Mathf.Deg2Rad)), (float)0.01, dist_slingshot * (Mathf.Sin(slingshot.transform.rotation.y*Mathf.Deg2Rad)));
                }

            }
        }
        else
        {
            ball_present=false;
            if(rubber_strain > 0f)
            {
                rubber_strain+= -rubber_force;
            }
        }

        transform.localScale = new Vector3(1,1,1+(rubber_strain*20/100));
    }
    void FixedUpdate()
    {
        // Ignore everything if this is another player's object
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
          return;
        }

        Quaternion rotation = gameObject.transform.root.rotation;
        float sinAngle = (float)Math.Sin(rotation.eulerAngles.y * ((Math.PI) / 180));
        float cosAngle = (float)Math.Cos(rotation.eulerAngles.y * ((Math.PI) / 180));

        if ((throw_ball == null) && ball_present)
        {
            Debug.Log("Ball instantiated by player");
            //throw_ball = PhotonNetwork.Instantiate(ballPrefab.name, new Vector3(0,4,0),Quaternion.identity, 0);
            throw_ball = Instantiate(ballPrefab, new Vector3(0, 4, 0), Quaternion.identity);
            throw_ball.transform.parent = this.transform.parent;
            bullseye = Instantiate(bullseyePrefab, new Vector3(0,0,0),Quaternion.identity);
            bullseye.transform.SetParent(throw_ball.transform.parent);
        }
        if(ball_present && (throw_ball != null))
        {
            throw_ball.transform.position = holder.transform.position;
            throw_ball.transform.localPosition+=new Vector3(0,-0.5f,0);
            float upvelocity = angle * 2 / throw_ball.GetComponent<Rigidbody>().mass;
            airtime = (upvelocity + Mathf.Sqrt((upvelocity*upvelocity) + (4 * gravity * holder.transform.position.y))) / (2 * gravity);
            float forwardvelocity = rubber_strain / (8 * throw_ball.GetComponent<Rigidbody>().mass);
            dist_slingshot = airtime * forwardvelocity;
            bullseye.transform.localPosition = new Vector3(bullseye.transform.localPosition.x,5-dist_slingshot,0);
            //bullseye.transform.position = new Vector3(dist_slingshot * (Mathf.Cos(slingshot.transform.rotation.y*Mathf.Deg2Rad)), (float)0.01, dist_slingshot * (Mathf.Sin(slingshot.transform.rotation.y*Mathf.Deg2Rad)));

        }
        if((!ball_present)&&(throw_ball != null))
        {
            impulse = new Vector3((rubber_strain/8)*sinAngle, angle, (rubber_strain/8)*cosAngle);
            throw_ball.GetComponent<Rigidbody>().AddRelativeForce(impulse,ForceMode.Impulse);
            Destroy(bullseye);
            throw_ball = null;
        }
    }
}

