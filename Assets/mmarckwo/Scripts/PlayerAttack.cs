using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // access mouse cam script to get camera pitch.
    public MouseCamLook playerCam;

    public GameObject attackSpell;
    public float shootSpeed = 700f;

    // 'castSpeed' is spell cooldown in seconds.
    // keep this at the bottom of public variables because of the header
    [Header("ACTUAL COOLDOWN SPEED IS: set value + 0.02.")]
    public float castSpeed = .28f;

    // vVv private variables vVv

    // timer doesn't track over maxTime seconds.
    private float timer;
    private float maxTime = 3f;

    private bool onCooldown = false;

    // Update is called once per frame
    void Update()
    {

        // when the player clicks and they're not on cooldown.
        if (Input.GetButtonDown("Fire1") && !onCooldown)
        {
            // set cooldown timer to 0.
            timer = 0.0f;
            onCooldown = true;

            // need to get spell type variable when more spells are added.

            // instantiate rotation arg combines player rotation and camera pitch.
            GameObject attack = Instantiate(attackSpell, transform.position, (transform.rotation * playerCam.lookAngle));
            attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);

        }


    }

    void FixedUpdate()
    {
        // this counts the time for the player in seconds.
        // timer is not dependent on framerate.
        // count time while timer is not over max time.
        if (timer <= maxTime)
        {
            // seconds are counted here.
            timer += Time.deltaTime;
            //Debug.Log(timer);

            // if timer is over cooldown time while player is on cooldown, set cooldown off.
            if ((timer >= castSpeed) && onCooldown)
            {
                onCooldown = false;
                //Debug.Log("cooldown reset.");
            }
        }   
    }
}
