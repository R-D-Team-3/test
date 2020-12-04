using System;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement; // temporary

using Photon.Pun;
using Photon.Realtime;
using System.Text;
using ExitGames.Client.Photon;
public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject controlPanel;

    [SerializeField]
    private Text feedbackText;

    [SerializeField]
    private byte maxPlayersPerRoom;

    bool isConnecting;
    bool isTeamBlue;
    public int minPlayers;
    string gameVersion = "1";

    [Space(10)]
    [Header("Custom Variables")]
    public InputField playerNameField;
    public InputField roomNameField;
    public Dropdown teampChoice;
    [Space(5)]
    public Text playerStatus;
    public Text connectionStatus;
    public Text playerList;

    [Space(5)]
    public GameObject roomJoinUI;
    public GameObject buttonLoadArena;
    public GameObject buttonJoinRoom;
    public Camera cam;
    string playerName = "";
    string roomName = "";

    // Start Method
    void Start()
    {
        cam.backgroundColor = new Color(227f / 149f, 121f / 255f, 102f / 255f);
        maxPlayersPerRoom = Convert.ToByte(PlayerPrefs.GetInt("max"));
        minPlayers = Convert.ToByte(PlayerPrefs.GetInt("min"));

        PlayerPrefs.DeleteAll();

        Debug.Log("Connecting to Photon Network");

        roomJoinUI.SetActive(false);
        buttonLoadArena.SetActive(false);
        playerList.gameObject.SetActive(false);

        ConnectToPhoton();
    }

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Helper Methods
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }
    public void setPlayerTeam(int team)
    {
        if (team==1)
        {
            Debug.Log("Team set as Blue");
            isTeamBlue = true;
            cam.backgroundColor = new Color(102f / 150f, 191f / 255f, 255f / 255f);
        }
        else
        {
            Debug.Log("Team set as Red");
            isTeamBlue = false;
            cam.backgroundColor = new Color(227f / 149f, 121f / 255f, 102f / 255f);
        }
    }
    // Methods
    void ConnectToPhoton()
    {
        connectionStatus.text = "Connecting...";
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void DisconnectToPhoton()
    {
        PhotonNetwork.Disconnect();
    }

    public void JoinRoom()
    {
        Hashtable playerinfo = new Hashtable();
        playerinfo.Add("team", isTeamBlue);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerinfo);
            Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + roomNameField.text);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayersPerRoom;
            TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
        }
    }

    public void LoadArena()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayers)
        {
            // TEMPORARY, CHANGE LATER!!!!!!!!!!!
            Hashtable roominfo = new Hashtable();
            roominfo.Add("RedPoints",0);
            roominfo.Add("BluePoints",0);
            Room thisroom = PhotonNetwork.CurrentRoom;
            Debug.Log("nullroom?"+(thisroom==null));
            thisroom.SetCustomProperties(roominfo);
            PhotonNetwork.LoadLevel("MPTestRoom");
            // SceneManager.LoadScene(0); // temporary
        }
        else
        {
            playerStatus.text = "Minimum 2 Players required to Load Arena!";
        }
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }

    public void BuildPlayerList()
    {
        StringBuilder sb = new StringBuilder("Available players: ");
        foreach (var name in PhotonNetwork.PlayerList)
        {
            sb.Append("\n");
            sb.Append(name);
        }
        playerList.text = sb.ToString();
    }

    // Photon Methods
    public override void OnConnected()
    {
        base.OnConnected();
        connectionStatus.text = "Connected to Photon!";
        connectionStatus.color = Color.green;
        roomJoinUI.SetActive(true);
        buttonLoadArena.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        controlPanel.SetActive(true);
        Debug.LogError("Disconnected. Please check your Internet connection.");
    }

    public override void OnJoinedRoom()
    {
        playerList.gameObject.SetActive(true);
        BuildPlayerList();///

        if (PhotonNetwork.IsMasterClient)
        {
            buttonLoadArena.SetActive(true);
            buttonJoinRoom.SetActive(false);
            playerStatus.text = "Your are Lobby Leader";
        }
        else
        {
            playerStatus.text = "Connected to Lobby";
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log(player + " joined the room!");
        BuildPlayerList();///
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        Debug.Log(player + " left the room...");
        BuildPlayerList();///
    }
}
