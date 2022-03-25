using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseSpell : MonoBehaviour
{

    public GameObject hitEffect;
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
        if(other.gameObject.tag == "Enemy")
        {
            Instantiate(hitEffect, other.transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(impulseHit, gameObject.transform.position);

            // get direction spell is facing (angle player shot it at), multiply by impulse force.
            force = transform.forward * forceStrength;

            // will need to replace player script with an enemy script for the other player. 
            other.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

            // destroy self when the spell hits the enemy.
            Destroy(this.gameObject);
        }
    }

}
