using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MPLauncher : MonoBehaviourPunCallbacks
{
    public int playerConnected = 0;
    //public TextMeshProUGUI txtNumConnected;
    public GameObject playerPrefab;
    public RoomListScript roomLister;

    [SerializeField] private GameObject screen1;
    [SerializeField] private GameObject screen2;

    private LoadBalancingClient loadBalancingClient;

    public void Connect()
    {
        //first outreach
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "1";
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    //client to master
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Lobby");

        screen1.SetActive(false);
        screen2.SetActive(true);
    }

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    //make room and become master client
    //    Debug.Log("Making room");
    //    PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, null);
    //}

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        //photonView.RPC("CountConnected", RpcTarget.AllBuffered);
        base.OnJoinedRoom();
        PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);

        ExitGames.Client.Photon.Hashtable setValue = new ExitGames.Client.Photon.Hashtable();
        setValue.Add("enemy", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(setValue);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //[PunRPC]
    //public void CountConnected()
    //{
    //    playerConnected++;
    //    txtNumConnected.SetText("Players connected: " + playerConnected.ToString());
    //
    //}

    public void StartGame()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable setValue = new ExitGames.Client.Photon.Hashtable();
            setValue.Add("enemy", true);
            PhotonNetwork.PlayerList[0].SetCustomProperties(setValue);
            Invoke("SwapScene", .25f);
        }
    }

    public void SwapScene()
    {
        // swtich to stage select.
        PhotonNetwork.LoadLevel("LevelSelect");
    }

    public void LoadSkyArena()
    {
        // swtich to sky arena.
        PhotonNetwork.LoadLevel("arena1");
    }

    public void LoadCastle()
    {
        // swtich to castle level.
        PhotonNetwork.LoadLevel("arena2");
    }

    public void LoadSciFi()
    {
        // swtich to castle level.
        PhotonNetwork.LoadLevel("arena3");
    }
}
