using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        other.gameObject.SendMessage("AttackHit");
    }
}
