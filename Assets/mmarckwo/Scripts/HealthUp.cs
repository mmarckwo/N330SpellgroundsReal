using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthUp : MonoBehaviourPun
{

    // on collide increase player health.
    // repeat same thing for other powerups.

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            this.photonView.RPC("DestroyItem", RpcTarget.All);
            other.SendMessage("HealthUp");
        }
    }

    [PunRPC]
    void DestroyItem()
    {
        Destroy(this.transform.parent.gameObject);
    }
}
