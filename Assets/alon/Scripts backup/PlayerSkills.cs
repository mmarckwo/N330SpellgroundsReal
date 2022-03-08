using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSkills : MonoBehaviourPun
{
    public GameObject trapPrefab;
    public GameObject trapLocation;
    private int Count;

    public void SetTrapCount()
    {
        if (!this.photonView.IsMine) return;
        Count = 0;
    }

    void Update()
    {
        if (!this.photonView.IsMine) return;
        
        if(Input.GetButtonDown("Fire1"))
        {
            Count++;

            if(Count < 4)
            {
                PhotonNetwork.Instantiate(this.trapPrefab.name, this.trapLocation.transform.position, Quaternion.identity);
            }
        }
    }
}
