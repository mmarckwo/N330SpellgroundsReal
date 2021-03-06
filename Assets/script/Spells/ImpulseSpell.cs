using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ImpulseSpell : MonoBehaviourPun
{

    private string hitEffect = "punch_effect";
    public string spellName = "Impulse";
    public float forceStrength = 45;

    public AudioClip impulseHit;

    private Vector3 force;

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

            // get direction spell is facing (angle player shot it at), multiply by impulse force.
            force = transform.forward * forceStrength;

            // push back the other player with created force value.
            other.gameObject.GetComponent<Player>().ImpulseHit(force);

            // destroy self and play sound when the spell hits the enemy.
            this.photonView.RPC("DestroySpellByHit", RpcTarget.All);
        }
    }

    [PunRPC]
    void DestroySpellByHit()
    {
        AudioSource.PlayClipAtPoint(impulseHit, gameObject.transform.position);
        Destroy(this.gameObject);
    }
}
