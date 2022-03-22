using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseSpell : MonoBehaviour
{

    public GameObject hitEffect;
    public string spellName = "Impulse";

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
        }
    }

}
