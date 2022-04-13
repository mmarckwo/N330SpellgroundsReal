using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
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
    [SerializeField] private int enemyScore = 0;
    private string playerSearchName;
    // needed for scoring system.
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private GameObject scoreTextObject;
    private TextMeshProUGUI scoreText;


    // Start is called before the first frame update
    void Start()

    {
        // get score tracker references.
        gameManagerObject = GameObject.Find("In-game Manager");
        gameManager = gameManagerObject.GetComponent<GameManager>();
        scoreTextObject = GameObject.Find("Canvas/Score Counter");
        scoreText = scoreTextObject.GetComponent<TextMeshProUGUI>();

        if (!this.photonView.IsMine)
        {
            this.GetComponentInChildren<AudioListener>().enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;

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
        if (!this.photonView.IsMine) return;

        // gravity.
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);

        // movement.
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector3 movement = new Vector3(straffe, 0, translation);

        //smoother = movement.magnitude;
        //Mathf.Clamp(smoother, 0, 1);

        movement.Normalize();
        //transform.Translate(movement * smoother);
		transform.Translate(movement * Mathf.Clamp(movement.magnitude, 0, 1));

        // check for jump availability.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void Update()
    {
        if (!this.photonView.IsMine) return;

        // free cursor with ESC.
        if (Input.GetKeyDown("escape"))
        {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }

        // lock the cursor back in when the game is clicked.
        if (Input.GetButtonDown("Fire1"))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // jump button. only jump when the player is touching the ground.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
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
        if (!this.photonView.IsMine) return;
        // play jump sound;
        jumpSound.Play();
        // player cannot cancel impulse attack by using addforce. (good thing)
        rb.AddForce(new Vector3(0, 1, 0) * jumpForce, ForceMode.Impulse);
        //rb.velocity = (new Vector3(0, 1, 0) * jumpForce);
    }

    // reset player stats and position after falling or dying.
    void Respawn()
    {
        if (!this.photonView.IsMine) return;
        gameObject.transform.position = respawnLocation.transform.position;
        health = maxHealth;
        speed = baseSpeed;
        HealthUpdate();
        rb.velocity = new Vector3(0, globalGravity, 0);

        this.photonView.RPC("PlayDeathSound", RpcTarget.All);
        this.photonView.RPC("UpdateScore", RpcTarget.All, this.gameObject.name);
    }

    [PunRPC]
    void UpdateScore(string playerInfo)
    {
        if (playerInfo == "EnemyPlayer")
        {
            Debug.Log("enemy perished");
            score += 1;
        }
        else if (playerInfo == "ClientPlayer")
        {
            Debug.Log("player perished");
            enemyScore += 1;
        }

        //UpdateCounter(playerInfo);
        this.photonView.RPC("UpdateCounter", RpcTarget.All);
    }

    [PunRPC]
    void UpdateCounter()
    {

        // player on their instance is named Player(Clone)
        // sigh.

        // get player name on their game instance.
        if (GameObject.Find("ClientPlayer"))
        {
            // if a ClientPlayer can be found in the game world, set the name ref to this.
            playerSearchName = "ClientPlayer";
        } 
        else
        {
            // if you're not the client, then you're the enemy.
            playerSearchName = "EnemyPlayer";
        }

        // only the master client takes care of the score. result of misfortune.
        // player search name is global for score updates to appear as intended on each player's screen.
        if (!PhotonNetwork.IsMasterClient) return;

        // player object name derived from game instance type (player or enemy).
        GameObject playerScoreObject = GameObject.Find(playerSearchName);
        enemyScore = playerScoreObject.GetComponent<Player>().enemyScore;
        Debug.Log(enemyScore);

        GameObject enemyScoreObject = GameObject.Find("Player(Clone)");
        try
        {
            score = enemyScoreObject.GetComponent<Player>().score;
            Debug.Log(score);
        } catch (Exception e)
        {
            Debug.Log("Enemy player does not exist.");
        }
        

        // update score references from each other on both players.
        try
        {
            enemyScoreObject.GetComponent<Player>().enemyScore = enemyScore;
        } catch (Exception e)
        {
            Debug.Log("Cannot update score on enemy player object because it doesn't exist.");
        }
        playerScoreObject.GetComponent<Player>().score = score;
        
        if(this.gameObject.name == "EnemyPlayer")
        {
            // scoreText.SetText();
        } 
        else
        {
            // scoreText.SetText();
        }

        this.photonView.RPC("RealScoreUpdate", RpcTarget.All, score, enemyScore);
    }

    [PunRPC]
    void RealScoreUpdate(int score, int enemyScore)
    {
        Debug.Log(playerSearchName);
        if(playerSearchName == "ClientPlayer")
        {
            // update score on screen from client POV.
            scoreText.SetText(score + " - " + enemyScore);
        } 
        else
        {
            // update score on screen from enemy POV.
            scoreText.SetText(enemyScore + " - " + score);
        }

        if (score == 3)
        {
            if(playerSearchName == "ClientPlayer")
            {
                // if you're the client player in the game and get 3 points you win, else you lose.
                gameManager.PlayerWin();
            }
            else
            {
                gameManager.EnemyWin();
            }
            
        }

        if (enemyScore == 3)
        {
            // if you're the enemy player in the game and you get 3 points you win, else you lose.
            if(playerSearchName == "EnemyPlayer")
            {
                gameManager.PlayerWin();
            }
            else
            {
                gameManager.EnemyWin();
            }
        }
    }

    //[PunRPC]
    // update health bar UI.
    public void HealthUpdate()
    {
        //if (!this.photonView.IsMine) return;
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

        // update health bar fill amount.
        healthBarFill.fillAmount = health / maxHealth;
    }

    [PunRPC]
    void PlayDeathSound()
    {
        deathSound.Play();
    }

    void HealthUp()
    {
        if (!this.photonView.IsMine) return;
        hpRestore.Play();

        // restore HP by some randomly decided value.
        health += 40.2f;
        HealthUpdate();

    }

    void HealthDown()
    {
        if (!this.photonView.IsMine) return;
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
        if (!this.photonView.IsMine) return;
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
        if (!this.photonView.IsMine) return;
        speed += 2.5f;
    }
}
