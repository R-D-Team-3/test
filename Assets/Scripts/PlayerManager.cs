﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Android;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPun
{
    public GameObject floatingTextPrefab;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    bool ball_present;
    public static GameObject LocalPlayerInstance;
    public GameObject ballPrefab;
    public GameObject bullseyePrefab;
    GameObject bullseye;
    GameObject throw_ball;
    GameObject rubber;
    GameObject holder;
    Vector3 impulse;
    Touch pulltouch;
    Vector2 pull_pos;
    Vector2 start_pos;
    float compass_input;
    float rubber_strain;
    float rubber_force;
    float angle;
    float sinAngle;
    float cosAngle;
    Vector3 oldPos;
    Vector3 newPos;
    Vector3 startPos;
    float latitude;
    float longitude;
    Boolean firstTime = true;
    Boolean isUpdating = false;
    float[] latBuffer = new float[5];
    float[] longBuffer = new float[5];
    int location = 0;
    int counter = 0;
    private Healthbar healthbarScript;
    [SerializeField]
    public Text notificationText;

    bool dead = false;
    bool deadTimerDone = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        ball_present = false;
        rubber_strain = 0f;
        rubber_force = 1f;
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        Input.location.Start();
        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
        rubber = this.transform.Find("SlingShot/Rubber").gameObject;
        holder = this.transform.Find("SlingShot/Rubber/holder").gameObject;
        if(rubber==null)
        {
            Debug.LogError("Find function does not work correctly.");
        }
    }

    private void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
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
        rubber.transform.localScale = new Vector3(1,1,1+(rubber_strain*20/100));
        compass_input = Input.compass.magneticHeading;

        Quaternion rotation = transform.rotation;
        sinAngle = (float)Math.Sin(rotation.eulerAngles.y * ((Math.PI) / 180));
        cosAngle = (float)Math.Cos(rotation.eulerAngles.y * ((Math.PI) / 180));
    }
    void FixedUpdate()
    {
        // Ignore everything if this is another player's object
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, compass_input, 0), Time.deltaTime * 3);

        oldPos = transform.position;
        transform.position = Vector3.MoveTowards(oldPos, startPos - (newPos * 80000), Time.deltaTime);

        if (!isUpdating)
        {
            StartCoroutine(GetLocation());
        }

        if ((throw_ball == null) && ball_present)
        {
            Debug.Log("Ball instantiated by Player");
            //throw_ball = Instantiate(ballPrefab, new Vector3(0, 4, 0), Quaternion.identity);
            throw_ball = PhotonNetwork.Instantiate(ballPrefab.name, this.transform.position + new Vector3(0, 3, 0), Quaternion.identity,0);
            Physics.IgnoreCollision(throw_ball.GetComponent<Collider>(), GetComponent<Collider>());
            throw_ball.transform.parent = this.transform;
            bullseye = PhotonNetwork.Instantiate(bullseyePrefab.name, this.transform.position,Quaternion.identity,0);
            bullseye.transform.parent = this.transform;
        }
        if(ball_present && (throw_ball != null))
        {
            throw_ball.transform.position = holder.transform.position;
            //throw_ball.GetComponent<Rigidbody>().mass=0;
            throw_ball.transform.localPosition+=new Vector3(0,-0.3f,0.5f);
            float airtime = (angle*2 + Mathf.Sqrt((angle*angle*4) + (40 * holder.transform.position.y))) /20;
            float forwardvelocity = rubber_strain / 8;
            float dist_slingshot = airtime * forwardvelocity;
            bullseye.transform.localPosition = new Vector3(0,0,0);
        }
        if((!ball_present)&&(throw_ball != null))
        {
            impulse = new Vector3((rubber_strain/8) * sinAngle, angle, (rubber_strain/8) * cosAngle);
            //impulse = new Vector3((rubber_strain/8), angle, 0);
            throw_ball.GetComponent<Rigidbody>().useGravity = true;
            throw_ball.GetComponent<Rigidbody>().AddRelativeForce(impulse,ForceMode.Impulse);
            bullseye.transform.SetParent(null,true);
            throw_ball.transform.SetParent(null, true);
            throw_ball = null;
            Destroy(bullseye, 1); // destroy after 1sec
        }

        //Death implementation
        GameObject obj = GameObject.Find("Healthbar1");
        healthbarScript = obj.GetComponent<Healthbar>();
        if (healthbarScript.health == healthbarScript.minimumHealth && !dead)
        {
            dead = true;
            StartCoroutine(showfloatingText());
            StartCoroutine(revive());
        }

    }
    IEnumerator showfloatingText()
    {
        Text text = GameObject.Find("deadTextAnchor").GetComponent<Text>();
        for (int i = 10; i>0; i--)
        {
            string newstring = "You died. Reviving in " + i + " seconds.";
            text.text = newstring;
            yield return new WaitForSeconds(1);
        }
        deadTimerDone = true;
        text.text = "";
    }

    IEnumerator revive()
    {
        while (!deadTimerDone)
        {
            //wait for showFloatingText
            yield return new WaitForSeconds(0.1f);
        }
        healthbarScript.GainHealth(100);
        deadTimerDone = false;
        dead = false;
        healthbarScript.ChangeHealthbarColor(new Color(0.35f, 1f, 0.35f));

    }

    IEnumerator GetLocation()
    {
        isUpdating = true;

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield return new WaitForSeconds(10); //Basically: sleep for 10 seconds (only works in coroutines!)
        // Start service before querying location
        Input.location.Start();
        // Wait until service initializes
        int maxWait = 100;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in 10 seconds
        if (maxWait < 1)
        {
            UnityEngine.Debug.Log("Timed out");
            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            UnityEngine.Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            LocationInfo gpsReadout = Input.location.lastData;


            latBuffer[location] = gpsReadout.latitude;
            longBuffer[location] = gpsReadout.longitude;
            location++;
            if (location == 5)
            {
                location = 0;
            }

            if (firstTime && counter < 30)
            {
                latitude = gpsReadout.latitude;
                longitude = gpsReadout.longitude;
                UnityEngine.Debug.Log("first");
                for (int i = 0; i < 5; i++)
                {
                    latBuffer[i] = latitude;
                    longBuffer[i] = longitude;

                }
                counter++;
                firstTime = false;
            }
            newPos = new Vector3(latitude - latBuffer.Average(), 0, longitude - longBuffer.Average());

            isUpdating = false;
        }
    }
}
