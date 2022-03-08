using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VictoryController : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        GameObject p = GameObject.FindWithTag("_player");
        PhotonNetwork.Destroy(p);
        Invoke("End", 5f);
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
             Cursor.lockState = CursorLockMode.None;
             Cursor.visible = true;
        }

    }

    public void End()
    {
        Application.Quit();
    }
}
