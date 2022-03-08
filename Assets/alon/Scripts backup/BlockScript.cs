using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BlockScript : MonoBehaviourPun
{
    GameObject blocker;
    private bool blocked = false;
    public GameObject doorRef;
    public Material lockedColor;
    public Material openColor;
    private AudioSource buttonSound;

    private void Start()
    {
        blocker = this.gameObject;
        buttonSound = this.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(this.photonView.IsMine)
        {
            buttonSound.Play();
            this.photonView.RPC("ToggleDoor", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ToggleDoor()
    {
        // blocker opens the door.
        if (blocked == false)
        {
            blocker.GetComponent<Renderer>().material = openColor;
            blocked = true;
        }
        // blocker locks the door.
        else
        {
            blocker.GetComponent<Renderer>().material = lockedColor;
            blocked = false;
        }

        doorRef.SendMessage("DoorResponse");
    }
}
