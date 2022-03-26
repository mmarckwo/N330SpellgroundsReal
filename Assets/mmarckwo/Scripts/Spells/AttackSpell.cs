using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackSpell : MonoBehaviourPun
{

    private string hitEffect = "punch_effect";
    public string spellName = "Attack";

    public AudioClip attackHit;

    private float timer = 0.0f;
    private float destroyTime = 1.0f;

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        // destroy self after destroyTime interval.
        if(timer > destroyTime)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!this.photonView.IsMine) return;
        if (other.gameObject.tag == "Enemy")
        {
            PhotonNetwork.Instantiate(hitEffect, other.transform.position, Quaternion.identity);

            // will need to replace PLAYER script component with an ENEMY script component for the other player. 
            // send message to other player?
            other.gameObject.GetComponent<Player>().health -= 14.5f;
            other.gameObject.GetComponent<Player>().HealthUpdate();

            // destroy self and play sound when the spell hits the enemy.
            this.photonView.RPC("DestroySpellByHit", RpcTarget.All);
        }
    }

    [PunRPC]
    void DestroySpellByHit()
    {
        AudioSource.PlayClipAtPoint(attackHit, gameObject.transform.position);
        Destroy(this.gameObject);
    }
}
