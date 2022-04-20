using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class CreateRoomButton : MonoBehaviourPunCallbacks
{

    [SerializeField] private TMP_InputField roomName;

    public void Click_CreateRoom()
    {
        Debug.Log("Making room");

        // if input is empty, set name to "Default"
        if (string.IsNullOrEmpty(roomName.text) || string.IsNullOrWhiteSpace(roomName.text))
        {
            roomName.text = "Default";
        }

        // create room with given name. if room already exists, then join that room instead.
        PhotonNetwork.JoinOrCreateRoom(roomName.text, new RoomOptions() { MaxPlayers = 2 }, null);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Made room with name of: " + roomName.text);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to make room");
    }
}
