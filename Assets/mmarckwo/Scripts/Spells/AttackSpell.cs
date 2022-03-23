using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpell : MonoBehaviour
{

    public GameObject hitEffect;
    public string spellName = "Attack";

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
            Debug.Log("hit enemy w/ attack");
            Instantiate(hitEffect, other.transform.position, Quaternion.identity);

            // will need to replace PLAYER script component with an ENEMY script component for the other player. 
            other.gameObject.GetComponent<Player>().health -= 14.5f;
            other.gameObject.GetComponent<Player>().HealthUpdate();

            // destroy self when the spell hits the enemy.
            Destroy(this.gameObject);
        }
    }
}
