﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Android;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerManager : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    static int MAX_POINTS = 300;
    public GameObject floatingTextPrefab;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    bool ball_present;
    public static GameObject LocalPlayerInstance;
    public GameObject ballPrefab;
    public GameObject bullseyePrefab;
 //   public GameObject tutorialButtonPrefab;
    public bool playerIsTeamBlue;
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
    public Healthbar healthbarScript;
    public BallAmount ballAmountScript;
    int myPoints;
    public bool dead = false;
    bool deadTimerDone = false;
    private int counter2 = 0;
    private float[] accelerometerBuffer = new float[150];
    private Boolean reload;
    int Player_ID;
    object[] teamlist;
    Text team_point_text;
    Text my_point_text;
    int teampoints;
    Hashtable resettable;
    public int tutorialIndex;
    bool tutorialButtonsDeleted = false;
    bool tutorial = true;
    // Start is called before the first frame update
    void Start()
    {
        resettable = new Hashtable();
        my_point_text = GameObject.Find("myPoints").GetComponent<Text>();
        team_point_text = GameObject.Find("teamPoints").GetComponent<Text>();
        PlayerPrefs.SetInt("dead", 0);
        PlayerPrefs.SetInt("tutorialIndex", 0);
        tutorialIndex = 0;

        myPoints = 0;
        teampoints = 0;
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
        if (rubber == null)
        {
            Debug.LogError("Find function does not work correctly.");
        }
    }

    private void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        Player_ID = this.gameObject.GetComponent<PhotonView>().ViewID;
        Debug.Log("PLayerID=" + Player_ID);
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
        if (Input.touchCount > 0 && dead == false)
        {
            pulltouch = Input.GetTouch(0);
            start_pos = pulltouch.rawPosition;
            if ((start_pos.x > Screen.width / 3) && (start_pos.x < 2 * Screen.width / 3) && (start_pos.y < Screen.height / 3))
            {
                pull_pos = pulltouch.position;
                if (pull_pos.y < start_pos.y)
                {
                    rubber_strain = (start_pos.y - pull_pos.y) * 400 / Screen.height;
                    rubber_force = rubber_strain / 10;
                    ball_present = true;
                }
            }
        }
        else
        {
            ball_present = false;
            if (rubber_strain > 0f)
            {
                rubber_strain += -rubber_force;
            }
        }
        rubber.transform.localScale = new Vector3(1, 1, 1 + (rubber_strain * 20 / 100));
        compass_input = Input.compass.magneticHeading;

        Quaternion rotation = transform.rotation;
        sinAngle = (float)Math.Sin(rotation.eulerAngles.y * ((Math.PI) / 180));
        cosAngle = (float)Math.Cos(rotation.eulerAngles.y * ((Math.PI) / 180));
        my_point_text.text = myPoints.ToString();
        team_point_text.text = teampoints.ToString();
        if (playerIsTeamBlue)
        {
            teampoints = (int)PhotonNetwork.CurrentRoom.CustomProperties["BluePoints"];
            if (teampoints >= MAX_POINTS)
            {
                resettable.Add("BluePoints", 0);
                PhotonNetwork.CurrentRoom.SetCustomProperties(resettable);
                dead = true;
                PlayerPrefs.SetInt("dead", 1);
                StartCoroutine(showfloatingText("You Won, restaring in ", false));
                StartCoroutine(revive());
            }
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["RedPoints"] >= MAX_POINTS)
            {
                resettable.Add("RedPoints", 0);
                PhotonNetwork.CurrentRoom.SetCustomProperties(resettable);
                dead = true;
                PlayerPrefs.SetInt("dead", 1);
                StartCoroutine(showfloatingText("You Lost, restaring in ", true));
                StartCoroutine(revive());
            }
        }
        else
        {
            teampoints = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedPoints"];
            if (teampoints >= MAX_POINTS)
            {
                resettable.Add("RedPoints", 0);
                PhotonNetwork.CurrentRoom.SetCustomProperties(resettable);
                dead = true;
                PlayerPrefs.SetInt("dead", 1);
                StartCoroutine(showfloatingText("You Won, restaring in ", false));
                StartCoroutine(revive());
            }
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["BluePoints"] >= MAX_POINTS)
            {
                resettable.Add("BluePoints", 0);
                PhotonNetwork.CurrentRoom.SetCustomProperties(resettable);
                dead = true;
                PlayerPrefs.SetInt("dead", 1);
                StartCoroutine(showfloatingText("You Lost, restaring in ", true));
                StartCoroutine(revive());
            }
        }
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] Data = info.photonView.InstantiationData;
        playerIsTeamBlue = (bool)Data[0];

        Debug.Log("Player " + Player_ID + "is in team blue? : " + playerIsTeamBlue);
    }
    public void incrementTutorialIndex()
    {
        int tutorial = PlayerPrefs.GetInt("tutorialIndex") + 1;
        PlayerPrefs.SetInt("tutorialIndex", tutorial);
    }
    void FixedUpdate()
    {
        //Tutorial implementation

        if ((tutorialButtonsDeleted == false) && (PhotonNetwork.CurrentRoom.PlayerCount > 1))
        {
            Destroy(GameObject.Find("tutorialNextButton"));
            Destroy(GameObject.Find("tutorialSkipButton"));
            tutorialButtonsDeleted = true;
            tutorial = false;
        }

        if ((PhotonNetwork.CurrentRoom.PlayerCount == 1) && (tutorial == true))
        {
            Text tutorialText = GameObject.Find("tutorialText").GetComponent<Text>();
            //nextButton = GameObject.Find("tutorialNextButton").getComponent<Button>();
            //nextButton.onClick.AddListener(incrementTutorialIndex);
            if (PlayerPrefs.GetInt("tutorialIndex") == 0)
            {

                tutorialText.text = "Welcome Special Agent. In a moment you will receive instructions for your upcoming mission.\n" +
              "Click the next button if you want to proceed.\n";
              
            }
            Debug.Log("tutorialIndex" + tutorialIndex);
            if (PlayerPrefs.GetInt("tutorialIndex") == 1)
            {
                tutorialText.text = "Before your mission starts, you will be assigned to one of two teams.\n\n" +
                    "The goal of your mission is to engage in combat and outscore the other team.\n\n" +
                    "Victory points are awarded for killing enemy team players and capturing antennas on your map.\n\n" +
                    "To truly test your survival skills a primitive weapon is used ... the slingshot";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 2)
            {
                tutorialText.text = "Currently there are no enemy units on the map. your Healthbar on the top of your screen is full. \n\n" +
                    "Once you are hit by an enemy player, life points will be deducted from the healthbar. \n\n" +
                    "if the Healthbar is empty, you need to return to your spawnpoint to revive after a time delay.\n\n" +
                    "Your spawnpoint is located at the place where you first entered the battleground.";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 3)
            {
                tutorialText.text = "Time for some action.\n\n" +
                    "in the top right corner of your screen you see the number of bullets you carry.\n" +
                    "if you lay your thumb on the sling, you take a bullet. a bullseye appears on the map.\n" +
                    "By stretching the sling and tilting your phone you can alter the strength of your shot.\n" +
                    "The aimed direction is determined by your phone's orientation.\n" +
                    "If you like where you are aiming at, lift your thumb and launch the bullet.\n\n" +
                    "Click next and try this out. ";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 4)
            {
                tutorialText.text = "";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 5)
            {
                tutorialText.text = "To reload you mimick picking up bullets from the ground.\n\n" +
                    "Hold your phone flat and horizontal while moving it down to the ground and back up.";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 6)
            {
                tutorialText.text = "You are now familiar with the shooting mechanic.\n\n" +
                    "another important objective is capturing and defending antennas.\n\n" +
                    "Move towards an antenna indicated on your map and click the antenna button once you are close.\n\n" +
                    "now click the next button.";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 7)
            {
                tutorialText.text = "\n\n\nTo capture this antenna, point your phone camera towards the Qr-code on the antennas real life location.\n\n" +
                    "Attention\n" +
                    "Enemies can still hit you while you capture an antenna.";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 8)
            {
                tutorialText.text = "when the Qr code is fully loaded you return to the battlefield.\n\n" +
                    "the antenna is now under your team's control. It keeps generating victory points until you lose it to the other team.";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 9)
            {
                tutorialText.text = "Your training is complete. Well done.\n\n" +
                    "you can now enter a multiplayer match or practice further on your own in here.";
            }
            if (PlayerPrefs.GetInt("tutorialIndex") == 10)
            {
                tutorialText.text = "";
                Destroy(GameObject.Find("tutorialNextButton"));
            }
        }
        // Ignore everything if this is another player's object
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, compass_input, 0), Time.deltaTime * 3);

        oldPos = transform.position;
        transform.position = Vector3.MoveTowards(oldPos, startPos - (newPos * 80000), Time.deltaTime);

        GameObject o = GameObject.Find("TextBallAmount");
        ballAmountScript = o.GetComponent<BallAmount>();

        if (!isUpdating)
        {
            StartCoroutine(GetLocation());
        }

        accelerometerBuffer[counter2] = (Input.acceleration.z * -10);
        counter2++;
        if (counter2 > 149)
        {
            counter2 = 0;
        }
        //Debug.Log(accelerometerBuffer.Average());

        if (accelerometerBuffer.Average() > 10.3 && !reload)
        {
            Debug.Log(PlayerPrefs.GetInt("max"));

            ballAmountScript.increment(1);
            reload = true;
            Invoke("waitOnReload", 2);
        }

        if ((throw_ball == null) && ball_present && ballAmountScript.getBallAmount() > 0)
        {
            Debug.Log("Ball instantiated by Player");
            //throw_ball = Instantiate(ballPrefab, new Vector3(0, 4, 0), Quaternion.identity);
            object[] Data = new object[1];
            Data[0] = (object)Player_ID;
            throw_ball = PhotonNetwork.Instantiate(ballPrefab.name, this.transform.position + new Vector3(0, 3, 0), Quaternion.identity, 0, Data);
            Physics.IgnoreCollision(throw_ball.GetComponent<Collider>(), GetComponent<Collider>());
            throw_ball.transform.parent = this.transform;
            bullseye = Instantiate(bullseyePrefab, this.transform.position, Quaternion.identity);
            bullseye.transform.parent = this.transform;
        }
        if (ball_present && (throw_ball != null))
        {
            throw_ball.transform.position = holder.transform.position;
            //throw_ball.GetComponent<Rigidbody>().mass=0;
            throw_ball.transform.localPosition += new Vector3(0, -0.3f, 0.5f);
            float airtime = (angle * 2 + Mathf.Sqrt((angle * angle * 4) + (40 * holder.transform.position.y))) / 20;
            float forwardvelocity = rubber_strain / 8;
            float dist_slingshot = airtime * forwardvelocity;
            bullseye.transform.localPosition = new Vector3(0, 0.1f, dist_slingshot);
        }
        if ((!ball_present) && (throw_ball != null))
        {
            impulse = new Vector3((rubber_strain / 8) * sinAngle, angle, (rubber_strain / 8) * cosAngle);
            //impulse = new Vector3((rubber_strain/8), angle, 0);
            throw_ball.GetComponent<Rigidbody>().useGravity = true;
            throw_ball.GetComponent<Rigidbody>().AddRelativeForce(impulse, ForceMode.Impulse);
            bullseye.transform.SetParent(null, true);
            throw_ball.transform.SetParent(null, true);
            ballAmountScript.decrement(1);
            throw_ball = null;
            Destroy(bullseye, 1); // destroy after 1sec
        }

        //Death implementation
        GameObject obj = GameObject.Find("Healthbar1");
        healthbarScript = obj.GetComponent<Healthbar>();
        healthbarScript.Player_ID = Player_ID;
        if (healthbarScript.health == healthbarScript.minimumHealth && !dead)
        {
            dead = true;
            PlayerPrefs.SetInt("dead", 1);
            StartCoroutine(showfloatingText("You died. Reviving in ", true));
            StartCoroutine(revive());
        }
    }
    public void Hits(int points, bool teamblue)
    {
        //if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        //{
        //  return;
        //}
        if (teamblue != playerIsTeamBlue)
        {
            myPoints = myPoints + points;
            Hashtable updatePoints = new Hashtable();
            int tp = 0;
            string key = "";
            if (playerIsTeamBlue)
            {
                tp = (int)PhotonNetwork.CurrentRoom.CustomProperties["BluePoints"];
                key = "BluePoints";
            }
            else
            {
                tp = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedPoints"];
                key = "RedPoints";
            }
            tp = tp + points;
            updatePoints.Add(key, tp);
            PhotonNetwork.CurrentRoom.SetCustomProperties(updatePoints);
        }
    }
    public void getHit(int points, bool teamblue)
    {
        if (teamblue != playerIsTeamBlue)
        {
            healthbarScript.TakeDamage(points);
        }
    }
    IEnumerator showfloatingText(string float_text, bool color)
    {
        Text text = GameObject.Find("deadTextAnchor").GetComponent<Text>();
        if (color)
        {
            text.color = Color.red;
        }
        else
        {
            text.color = Color.green;
        }
        for (int i = 10; i > 0; i--)
        {
            string newstring = float_text + i + " seconds.";
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
        PlayerPrefs.SetInt("dead", 0);
        healthbarScript.ChangeHealthbarColor(new Color(0.35f, 1f, 0.35f));

    }
    void waitOnReload()
    {
        reload = false;
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