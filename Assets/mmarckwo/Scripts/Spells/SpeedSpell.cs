using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSpell : MonoBehaviour
{

    public GameObject hitEffect;
    public string spellName = "Speed";

    public AudioClip speedHit;

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
            Debug.Log("hit enemy w/ speed");
            Instantiate(hitEffect, other.transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(speedHit, gameObject.transform.position);

            // will need to replace player script with an enemy script for the other player. 
            // increase the enemy's speed.
            other.gameObject.GetComponent<Player>().speed += 2.5f;

            // destroy self when the spell hits the enemy.
            Destroy(this.gameObject);
        }
    }

}
