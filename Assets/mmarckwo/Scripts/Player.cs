using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks,IPunInstantiateMagicCallback
{

    public float maxHealth = 10.0f;
    [Header("Do not change this")]
    public float health;

    [Header("Physics")]
    public float jumpForce = 10.0f;

    public float baseSpeed = 10.0f;
    public float speed = 10.0f;

    // player will use its own gravity for jump physics.
    public float gravityScale = 1.0f;
    public static float globalGravity = -9.81f;

    [Header("Jump Checking & Sound")]
    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;
    private bool isGrounded;
    public AudioSource jumpSound;

    [Header("Respawner")]
    private GameObject respawner;
    private Transform respawnLocation;

    [Header("Health Bar Properties")]
    private GameObject healthBar;
    private Image healthBarFill;
    public Color goodHealth = new Color(69, 255, 137);
    public Color lowHealth = new Color(255, 0, 85);
    public float healthLerpSpeed = 5;       // higher lerp speed goes faster.

    [Header("HP Sounds")]
    public AudioSource hpRestore;
    public AudioSource hpDrain;
    public AudioSource deathSound;

    private Rigidbody rb;

    [Header("Score Stuff")]
    // score for the game, get to three to win.
    // host player score.
    [SerializeField] private int score = 0;
    // get score from enemy.
	
	[HideInInspector, SerializeField] public bool isMaster;
	
    public GameManager gameManager;
	
    // needed for scoring system.
    private GameObject scoreTextObject;
    private TextMeshProUGUI scoreText;


    // Start is called before the first frame update
    void Start()
    {
        // get score tracker references.
        gameManager = GameObject.Find("In-game Manager").GetComponent<GameManager>();
        scoreTextObject = GameObject.Find("Canvas/Score Counter");
        scoreText = scoreTextObject.GetComponent<TextMeshProUGUI>();

        if (!this.photonView.IsMine)
        {
            this.GetComponentInChildren<AudioListener>().enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Confined;

        // find respawn location in scene hierarchy, set respawn location to transform of object.
        respawner = GameObject.Find("Respawn Location");
        respawnLocation = respawner.transform;

        // find health bar from UI in scene hierarchy.
        healthBar = GameObject.Find("Canvas/HealthBar/HealthBarInner");
        // get fill image of health bar.
        healthBarFill = healthBar.GetComponent<Image>();

        // get rigidbody component, use custom gravity.
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // set player to be at full HP.
        health = maxHealth;

        // set player to be at base speed.
        speed = baseSpeed;
    }

    void FixedUpdate()
    {
        // comment this out when testing single player.
        if (gameManager.ShouldntUpdate(this)) return;

        // gravity.
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);

        // movement.
        float translation = Input.GetAxis("Vertical");
        float straffe = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(straffe, 0, translation);
		
		float movementMagnitudeSquared = movement.sqrMagnitude;
		
		if(movementMagnitudeSquared > 1.0f){
			
			movement /= Mathf.Sqrt(movementMagnitudeSquared);
			
		}
		
		movement *= speed;
		movement *= Time.deltaTime;
		
		transform.Translate(movement);

        // check for jump availability.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void Update()
    {
        // player only updates themselves.
        if (!this.photonView.IsMine) return;

        // animate HP bar fill.
        HPLerp();

        // free cursor with ESC.
        if (Input.GetKeyDown("escape"))
        {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }

        // lock the cursor back in when the game is clicked.
        if (Input.GetButtonDown("Fire1"))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        if (gameManager.ShouldntUpdate(this)) return;

        // jump button. only jump when the player is touching the ground.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
			isGrounded = false;
        }

        // respawn the player after going below 80 in the world.
        if(gameObject.transform.position.y <= -80)
        {
            Respawn();
            //UpdateScore(this.gameObject.tag);
            //this.photonView.RPC("UpdateScore", RpcTarget.All, this.gameObject.name);
        }
    }

    void Jump()
    {
		if (gameManager.ShouldntUpdate(this)) return;
        // play jump sound;
        jumpSound.Play();
        // player cannot cancel impulse attack by using addforce. (good thing)
        rb.AddForce(new Vector3(0, 1, 0) * jumpForce, ForceMode.Impulse);
        //rb.velocity = (new Vector3(0, 1, 0) * jumpForce);
    }

    // reset player stats and position after falling or dying.
    void Respawn()
    {
        if (gameManager.ShouldntUpdate(this)) return;
        gameObject.transform.position = respawnLocation.transform.position;
        health = maxHealth;
        speed = baseSpeed;
        HealthUpdate();
        rb.velocity = new Vector3(0, globalGravity, 0);

        this.photonView.RPC("PlayDeathSound", RpcTarget.All);
        this.photonView.RPC("IncrementScore", RpcTarget.All, this.isMaster);
    }

    [PunRPC]
    void IncrementScore(bool incrementMaster)
    {
        if(incrementMaster){
			
            Debug.Log("master perished");
            gameManager.serverPlayer.score++;
			
        }else{
			
            Debug.Log("server perished");
            gameManager.masterPlayer.score++;
			
        }
		
		if(gameManager.playerIsMaster){
				
			ScoreUpdate(gameManager.masterPlayer.score,gameManager.serverPlayer.score);
			
		}else{
			
			ScoreUpdate(gameManager.serverPlayer.score,gameManager.masterPlayer.score);
			
		}
		
		if(gameManager.serverPlayer.score == 3){
			
			gameManager.EndScreen(!this.isMaster);
			
		}else if(gameManager.masterPlayer.score == 3){
			
			gameManager.EndScreen(this.isMaster);
			
		}
		
    }

    void ScoreUpdate(int leftScore, int rightScore)
    {
		
		scoreText.SetText(leftScore + " - " + rightScore);
		
    }

    //[PunRPC]
    // update health bar UI.
    public void HealthUpdate()
    {
        // clamp health to not go above max HP.
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        // make health bar green again if player recovers enough HP.
        if (health / maxHealth > .30)
        {
            healthBarFill.color = goodHealth;
        }

        // clamp health to not go below 0.
        if (health < 0)
        {
            health = 0;
        }

        // make the health bar red when the player is at low HP.
        if ((health / maxHealth <= .30) || (health == 1))
        {
            healthBarFill.color = lowHealth;
        }

        // respawn when dead.
        if (health == 0)
        {
            Respawn();
            //UpdateScore(this.gameObject.tag);
            //this.photonView.RPC("UpdateScore", RpcTarget.All, this.gameObject.name);
        }
    }

    void HPLerp()
    {
        // goes in Update() to animate lerp.
        // update health bar fill amount.
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, (health / maxHealth), Time.deltaTime * healthLerpSpeed);
    }

    [PunRPC]
    void PlayDeathSound()
    {
        deathSound.Play();
    }

    void HealthUp()
    {
		if (gameManager.ShouldntUpdate(this)) return;
        hpRestore.Play();

        // restore HP by some randomly decided value.
        health += 40.2f;
        HealthUpdate();

    }

    void HealthDown()
    {
		if (gameManager.ShouldntUpdate(this)) return;
        hpDrain.Play();

        // decrease health by arbitrarily decided value.
        health -= 15.2f;
        //this.photonView.RPC("HealthUpdate", RpcTarget.All);
        HealthUpdate();
    }

    public void AttackHit()
    {
        this.photonView.RPC("ReceiveAttackHit", RpcTarget.All);
    }

    [PunRPC]
    void ReceiveAttackHit()
    {
		if (gameManager.ShouldntUpdate(this)) return;
        health -= 14.5f;
        // rpc??? NO.
        HealthUpdate();
    }

    public void ImpulseHit(Vector3 force)
    {
        this.photonView.RPC("TakeKnockback", RpcTarget.All, force);
    }

    [PunRPC]
    void TakeKnockback(Vector3 force)
    {
        this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    public void IncreaseSpeed()
    {
        this.photonView.RPC("IncreaseSpeedHit", RpcTarget.All);
    }

    [PunRPC]
    void IncreaseSpeedHit()
    {
		if (gameManager.ShouldntUpdate(this)) return;
        speed += 2.5f;
    }
	
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		
		gameManager = GameObject.Find("In-game Manager").GetComponent<GameManager>();
		
		if(this.photonView.IsMine) return;
		
		if(this.isMaster){
			
			gameManager.masterPlayer = this;
			
		}else{
			
			gameManager.serverPlayer = this;
			
		}
		
	}
}
