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

        #endregion

        private void Start()
        {
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
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(-6f, 0f, 0f), Quaternion.identity, 0);
                    } else
                    {
                        Debug.LogFormat("We are Instantiating LocalPlayer");
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(6f, 0f, 0f), Quaternion.identity, 0);
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
        }
        #endregion
    }
}