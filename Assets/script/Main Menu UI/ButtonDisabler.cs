using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ButtonDisabler : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // if we're the client, disable the button.
        if (PhotonNetwork.IsMasterClient) return;
        GetComponent<Button>().enabled = false;
        GetComponent<Image>().color = Color.gray;
    }

}
