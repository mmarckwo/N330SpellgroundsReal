using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthDown : MonoBehaviourPun
{

    // on collide increase player health.
    // repeat same thing for other powerups.

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.SendMessage("HealthDown");
            this.photonView.RPC("DestroyItem", RpcTarget.All);
            
        }
    }

    [PunRPC]
    void DestroyItem()
    {
        Destroy(this.transform.parent.gameObject);
    }
}
