using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    // access mouse cam script to get camera pitch.
    public MouseCamLook playerCam;
    // get camera x rotation to rotate model.
    //private float playerXRotation;

    [Header("Magic Attacks")]
    private string attackSpell = "AttackSpell";
    private string impulseSpell = "ImpulseSpell";
    private string speedSpell = "SpeedSpell";
    public float shootSpeed = 700f;
    private int spellSelect = 1;        // default to attack spell.
    private GameObject attack;

    [Header("Spell indicator icons")]
    private TextMeshProUGUI attackIndicator;
    private TextMeshProUGUI impulseIndicator;
    private TextMeshProUGUI speedIndicator;
    private GameObject attackIndicatorObject;
    private GameObject impulseIndicatorObject;
    private GameObject speedIndicatorObject;

    [Header("Sounds")]
    public AudioSource attackSound;
    public AudioSource impulseSound;
    public AudioSource speedSound; 

    // 'castSpeed' is spell cooldown in seconds.
    // keep this at the bottom of public variables because of the header
    [Header("ACTUAL COOLDOWN SPEED IS: set value + 0.02.")]
    public float castSpeed = .28f;

    // vVv private variables vVv

    // timer doesn't track over maxTime seconds.
    private float timer;
    private float maxTime = 3f;

    private bool onCooldown = false;

    private void Start()
    {
        if (!this.photonView.IsMine) return;

        // find spell indicator refrences in scene hierarchy.
        attackIndicatorObject = GameObject.Find("Canvas/Spell Icons/Attack Spell/Attack Indicator");
        impulseIndicatorObject = GameObject.Find("Canvas/Spell Icons/Impulse Spell/Impulse Indicator");
        speedIndicatorObject = GameObject.Find("Canvas/Spell Icons/Speed Spell/Speed Indicator");

        // set TMP text refs.
        attackIndicator = attackIndicatorObject.GetComponent<TextMeshProUGUI>();
        impulseIndicator = impulseIndicatorObject.GetComponent<TextMeshProUGUI>();
        speedIndicator = speedIndicatorObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!this.photonView.IsMine) return;
        // check for when the player switches spells and set the indicators on the icons to bold & italic.
        if (Input.GetKeyDown("1"))
        {
            spellSelect = 1;
            attackIndicator.fontStyle = FontStyles.Bold | FontStyles.Italic;
            impulseIndicator.fontStyle = FontStyles.Normal;
            speedIndicator.fontStyle = FontStyles.Normal;
        }

        if (Input.GetKeyDown("2"))
        {
            spellSelect = 2;
            attackIndicator.fontStyle = FontStyles.Normal;
            impulseIndicator.fontStyle = FontStyles.Bold | FontStyles.Italic;
            speedIndicator.fontStyle = FontStyles.Normal;
        }

        if (Input.GetKeyDown("3"))
        {
            spellSelect = 3;
            attackIndicator.fontStyle = FontStyles.Normal;
            impulseIndicator.fontStyle = FontStyles.Normal;
            speedIndicator.fontStyle = FontStyles.Bold | FontStyles.Italic;
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
                    attack = PhotonNetwork.Instantiate(attackSpell, transform.position, (transform.rotation * playerCam.lookAngle));
                    attackSound.Play();
                    attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);
                    break;
                case 2:
                    attack = PhotonNetwork.Instantiate(impulseSpell, transform.position, (transform.rotation * playerCam.lookAngle));
                    impulseSound.Play();
                    attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);
                    break;
                case 3:
                    attack = PhotonNetwork.Instantiate(speedSpell, transform.position, (transform.rotation * playerCam.lookAngle));
                    speedSound.Play();
                    attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);
                    break;
                default:
                    Debug.Log("default attack");
                    attack = PhotonNetwork.Instantiate(attackSpell, transform.position, (transform.rotation * playerCam.lookAngle));
                    attackSound.Play();
                    attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);
                    break;
            }

        }

        // attempt to get player camera x rotation to rotate player model. idk.
        //playerXRotation = playerCam.GetComponent<Transform>().eulerAngles.y;
        //transform.rotation *= playerCam.lookAngle;
        //Debug.Log(playerCam.lookAngle);
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
