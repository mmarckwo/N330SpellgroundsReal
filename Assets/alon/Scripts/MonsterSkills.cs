using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonsterSkills : MonoBehaviourPun
{
    public GameObject HitBox;
    private float FiringRate = 2f;
    private float NextFire;


    void Update()
    {
        if (!this.photonView.IsMine) return;
        
        
        if(Input.GetButtonDown("Fire1") && Time.time > NextFire)
        {
            this.photonView.RPC("Attack", RpcTarget.All);
            NextFire = Time.time + FiringRate;
        }
        else
        {
        this.photonView.RPC("AttackFinish", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Attack()
    {
        this.HitBox.SetActive(true);
    }

    [PunRPC]
    public void AttackFinish()
    {
        this.HitBox.SetActive(false);
    }
}
