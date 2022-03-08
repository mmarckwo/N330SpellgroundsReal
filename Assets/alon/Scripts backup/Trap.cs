using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Trap : MonoBehaviourPun
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "ClientPlayer") 
        {
            if(PhotonNetwork.IsMasterClient)
            {
                other.gameObject.SendMessage("StunHit");
                this.photonView.RPC("RemoveTrap", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void RemoveTrap()
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
