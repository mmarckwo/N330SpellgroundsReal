using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // access mouse cam script to get camera pitch.
    public MouseCamLook playerCam;

    public GameObject attackSpell;
    public GameObject impulseSpell;
    public GameObject speedSpell;
    public float shootSpeed = 700f;
    private int spellSelect = 1;        // default to attack spell.
    private GameObject attack;

    public ParticleSystem punchEffect;

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

        // check for when the player switches spells.
        if (Input.GetKeyDown("1"))
        {
            spellSelect = 1;
        }

        if (Input.GetKeyDown("2"))
        {
            spellSelect = 2;
        }

        if (Input.GetKeyDown("3"))
        {
            spellSelect = 3;
        }

        // when the player clicks and they're not on cooldown.
        if (Input.GetButtonDown("Fire1") && !onCooldown)
        {
            // set cooldown timer to 0.
            timer = 0.0f;
            onCooldown = true;

            // switch statement that shoots whatever spell the player has selected.
            // instantiate rotation arg combines player rotation and camera pitch.
            switch(spellSelect)
            {
                case 1:
                    Debug.Log("attack");
                    attack = Instantiate(attackSpell, transform.position, (transform.rotation * playerCam.lookAngle));
                    attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);
                    break;
                case 2:
                    Debug.Log("impulse");
                    attack = Instantiate(impulseSpell, transform.position, (transform.rotation * playerCam.lookAngle));
                    attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);
                    break;
                case 3:
                    Debug.Log("speed");
                    attack = Instantiate(speedSpell, transform.position, (transform.rotation * playerCam.lookAngle));
                    attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);
                    break;
                default:
                    Debug.Log("default");
                    attack = Instantiate(attackSpell, transform.position, (transform.rotation * playerCam.lookAngle));
                    attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);
                    break;
            }

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
