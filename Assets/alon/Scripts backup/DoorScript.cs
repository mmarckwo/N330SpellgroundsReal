using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorScript : MonoBehaviourPun
{
    private AudioSource doorSound;

    public GameObject doorCollider;

    private void Start()
    {
        doorSound = this.GetComponent<AudioSource>();
    }

    public void DoorResponse()
    {
        doorSound.Play();
        DoorToggle();
    }

    //[PunRPC]
    public void DoorToggle()
    {
        doorCollider.SetActive(!doorCollider.activeSelf);
    }

}
