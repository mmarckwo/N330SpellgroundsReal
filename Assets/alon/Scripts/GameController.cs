using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks
{
    public float gameDuration;
    public Image timeBar;
    public Text txtRole;
    private float gameOverTime;
    public Camera theCamera;
    public Transform[] spawnPoints;
    private int requiredPlayersDeadCount;
    private int deathCount = 0;
    private GameObject m;

    public void Start()
    {   
        theCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        this.theCamera.SendMessage("TurnOnCamera");
        this.gameOverTime = Time.time + this.gameDuration;
        requiredPlayersDeadCount = PhotonNetwork.PlayerList.Length - 1;
        Debug.Log("requiredPlayersDeadCount: " + requiredPlayersDeadCount);
    }

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            m = GameObject.Find("ClientPlayer");
            m.SendMessage("StartGame");

            if(m)
            {
                txtRole.text = "You are: Salesman";
            }
            else
            {
                txtRole.text = "You are: Grandma";
            }
            this.photonView.RPC("ServerSetPosition", RpcTarget.All);
        }
    }

    [PunRPC]
    void ServerSetPosition()
    {
        GameObject p = GameObject.FindWithTag("_player");
        int randomPos = Random.Range(0, spawnPoints.Length);
        Debug.Log(randomPos);
        p.transform.position = spawnPoints[randomPos].position;
        p.SendMessage("SetTrapCount");
    }

    public void DeathCount()
    {
        deathCount++;
        Debug.Log("deathCount " + deathCount);
    }

    void Update() 
    {
        //allows mouse to move
        if (Input.GetKey("escape"))
        {
             Cursor.lockState = CursorLockMode.None;
             Cursor.visible = true;
        }

        timeBar.fillAmount = 1 - (Time.time / this.gameOverTime);

        if (deathCount == requiredPlayersDeadCount)
        {
            Invoke("SwapSceneEvil", 3f);
        }
        if(Time.time > this.gameOverTime)
        {
            Invoke("SwapSceneGood", 3f);
        }
    }

    public void SwapSceneEvil()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("VictoryEvilScene");
            PhotonNetwork.Destroy(this.gameObject);
        }
        else
        {
            return;
        }
    }

    public void SwapSceneGood()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("VictoryGoodScene");       
            PhotonNetwork.Destroy(this.gameObject);
        }
        else
        {
            return;
        }
    }

}
