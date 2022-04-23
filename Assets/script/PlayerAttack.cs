using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    // access mouse cam script to get camera pitch.
    //public MouseCamLook playerCam;
    // get camera x rotation to rotate model.
    //private float playerXRotation;

    [Header("Magic Attacks")]
    private string attackSpell = "AttackSpell";
    private string impulseSpell = "ImpulseSpell";
    private string speedSpell = "SpeedSpell";
    public float shootSpeed = 12f;
	public float chargedShootSpeed;
    private int spellSelect = 1;        // default to attack spell.

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
	public float chargeSpeed = 1.0f;
	
    // vVv private variables vVv

	private float maxTime = 3f;
    private float cooldownTimer; //Tracks time since the last time a shot was fired. Doesn't track over maxTime seconds.
	[HideInInspector]
    public float chargeTimer; //Tracks how long we've been holding down the fire button. Doesn't track over maxTime seconds.

    private bool onCooldown = false;
	
	private Player player; //reference to player script
	public MouseCamLook mouseCamLook; //reference to mouseCamLook script

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
		
		//get player, mouseCamLook scripts
		this.player = GetComponent<Player>();
		
    }
	
	void ShootSpell(bool charged,string prefabName){
		
		GameObject attack = PhotonNetwork.Instantiate(prefabName, transform.position, (transform.rotation));
		attackSound.Play();
		
		Rigidbody rb = attack.GetComponent<Rigidbody>();
		
		float angle = this.mouseCamLook.GetInaccurateAngle();
		
		float usedShootSpeed;
		
		if(charged){
			
			usedShootSpeed = chargedShootSpeed;
			
		}else{
			
			usedShootSpeed = shootSpeed;
			
		}
		
		Vector3 shootDireciton = new Vector3(usedShootSpeed*Mathf.Sin(angle), 0, usedShootSpeed*Mathf.Cos(angle));
		
		rb.AddRelativeForce(shootDireciton,ForceMode.Impulse);
		rb.AddForce(player.velocity,ForceMode.Impulse);
		
		//Debug.Log(angle);
		
	}
	
	void ShootSpell(bool charged){
		
		// switch statement that shoots whatever spell the player has selected.
		// instantiate rotation arg combines player rotation and camera pitch.
		switch(spellSelect)
		{
			case 1:
				ShootSpell(charged,attackSpell);
				break;
			case 2:
				ShootSpell(charged,impulseSpell);
				break;
			case 3:
				ShootSpell(charged,speedSpell);
				break;
			default:
				Debug.Log("default attack");
				ShootSpell(charged,attackSpell);
				break;
		}
		
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
            // set cooldown cooldownTimer to 0.
            cooldownTimer = 0.0f;
            onCooldown = true;

            ShootSpell(false);
			
        }else if(chargeTimer > chargeSpeed){
			
			chargeTimer = 0.0f;
			
			ShootSpell(true);
			
		}
		
	}

    void FixedUpdate()
    {
        // this counts the time for the player in seconds.
        // cooldownTimer is not dependent on framerate.
        // count time while cooldownTimer is not over max time.
        if (cooldownTimer <= maxTime)
        {
            // seconds are counted here.
            cooldownTimer += Time.deltaTime;
            //Debug.Log(cooldownTimer);

            // if cooldownTimer is over cooldown time while player is on cooldown, set cooldown off.
            if ((cooldownTimer >= castSpeed) && onCooldown)
            {
                onCooldown = false;
                //Debug.Log("cooldown reset.");
            }
			
        }
		
		if(chargeTimer <= maxTime && Input.GetButton("Fire1")){
			
			chargeTimer += Time.deltaTime;
			
		}else{
			
			chargeTimer = 0.0f;
			
		}
		
    }
}
