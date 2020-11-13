using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement; // temporary

using Photon.Pun;
using Photon.Realtime;
using System.Text;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject controlPanel;

    [SerializeField]
    private Text feedbackText;

    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    bool isConnecting;

    string gameVersion = "1";

    [Space(10)]
    [Header("Custom Variables")]
    public InputField playerNameField;
    public InputField roomNameField;

    [Space(5)]
    public Text playerStatus;
    public Text connectionStatus;
    public Text playerList;

    [Space(5)]
    public GameObject roomJoinUI;
    public GameObject buttonLoadArena;
    public GameObject buttonJoinRoom;

    string playerName = "";
    string roomName = "";

    // Start Method
    void Start()
    {
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

    // Methods
    void ConnectToPhoton()
    {
        connectionStatus.text = "Connecting...";
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + roomNameField.text);
            RoomOptions roomOptions = new RoomOptions();
            TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
        }
    }

    public void LoadArena()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            // TEMPORARY, CHANGE LATER!!!!!!!!!!!
            PhotonNetwork.LoadLevel("MPTestRoom");
            // SceneManager.LoadScene(0); // temporary
        }
        else
        {
            playerStatus.text = "Minimum 2 Players required to Load Arena!";
        }
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
