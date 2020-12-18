using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.Team3.Game
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public GameObject healthPrefab;
        public GameObject ammoPrefab;
        public bool blueteam;
        #endregion

        private void Start()
        {
            object[] playerinfo = new object[1];
            blueteam = (bool)PhotonNetwork.LocalPlayer.CustomProperties["team"]; // isTeamBlue==true
            playerinfo[0] = (object)blueteam;
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Debug.LogFormat("We are Instantiating LocalPlayer");
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(-3f, 0f, 0f), Quaternion.identity, 0,playerinfo);
                        PhotonNetwork.InstantiateRoomObject(this.healthPrefab.name, new Vector3(40, 0.8f, 200), Quaternion.identity);
                        PhotonNetwork.InstantiateRoomObject(this.healthPrefab.name, new Vector3(1, 0.8f, 104), Quaternion.identity);
                        PhotonNetwork.InstantiateRoomObject(this.healthPrefab.name, new Vector3(30, 0.8f, 15), Quaternion.identity);
                        PhotonNetwork.InstantiateRoomObject(this.healthPrefab.name, new Vector3(-20, 0.8f, -60), Quaternion.identity);
                        PhotonNetwork.InstantiateRoomObject(this.ammoPrefab.name, new Vector3(20, 0.8f, 106), Quaternion.identity);
                        PhotonNetwork.InstantiateRoomObject(this.ammoPrefab.name, new Vector3(4, 0.8f, 201), Quaternion.identity);
                        PhotonNetwork.InstantiateRoomObject(this.ammoPrefab.name, new Vector3(-16, 0.8f, 36), Quaternion.identity);
                        PhotonNetwork.InstantiateRoomObject(this.ammoPrefab.name, new Vector3(22, 0.8f, -80), Quaternion.identity);
                    } else
                    {
                        Debug.LogFormat("We are Instantiating LocalPlayer");
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(UnityEngine.Random.Range(-1f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f)), Quaternion.identity, 0,playerinfo);
                    }

                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                } 
            }
        }

        #region Photon Callbacks
        // Is called when player leaves room
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Launcher");
        }
        #endregion

        #region Public Methods
        // Press button to leave room
        public void LeaveRoom() 
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Menu");
        }
        #endregion
    }
}