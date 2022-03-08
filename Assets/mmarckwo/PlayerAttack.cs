using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // access mouse cam script to get camera pitch.
    public MouseCamLook playerCam;

    public GameObject attackSpell;
    public float shootSpeed = 700f;

    // Update is called once per frame
    void Update()
    {
        // change this to a method.
        if (Input.GetButtonDown("Fire1"))
        {

            // instantiate rotation arg combines player rotation and camera pitch.
            GameObject attack = Instantiate(attackSpell, transform.position, (transform.rotation * playerCam.lookAngle));
            attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);

        }
    }
}
