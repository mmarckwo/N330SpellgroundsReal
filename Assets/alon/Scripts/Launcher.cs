using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks 
{
    public int playerConnected = 0;
    public Text txtNumConnected;
    public GameObject playerPrefab;

    public void Connect() 
    {
        //first outreach
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //client to master
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected as Client");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //make room and become master client
        Debug.Log("Making room");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joining room");
        photonView.RPC("CountConnected", RpcTarget.AllBuffered);
        base.OnJoinedRoom();
        GameObject p = null;
        p = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0,310,0), Quaternion.identity);
        p.name = "ClientPlayer";
        PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);

        ExitGames.Client.Photon.Hashtable setValue = new ExitGames.Client.Photon.Hashtable();
        setValue.Add("monster", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(setValue);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    [PunRPC]
    public void CountConnected()
    {
        playerConnected++;
        txtNumConnected.text = "Connected  Players: " + playerConnected.ToString();

    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable setValue = new ExitGames.Client.Photon.Hashtable();
            setValue.Add("monster", true);
            PhotonNetwork.PlayerList[0].SetCustomProperties(setValue);
            Invoke("SwapScene", 5f);
        }
    }

    public void SwapScene()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
