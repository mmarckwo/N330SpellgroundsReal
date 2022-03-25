using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : MonoBehaviour
{

    // on collide increase player health.
    // repeat same thing for other powerups.

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Destroy(this.transform.parent.gameObject);
            other.SendMessage("HealthUp");
        }
    }
}
