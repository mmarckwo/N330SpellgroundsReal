using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseSpell : MonoBehaviour
{

    public GameObject hitEffect;
    public string spellName = "Impulse";

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
            Debug.Log("hit enemy w/ impulse");
            Instantiate(hitEffect, other.transform.position, Quaternion.identity);

            force = new Vector3(0, 0, 45);

            // will need to replace player script with an enemy script for the other player. 
            other.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
    }

}
