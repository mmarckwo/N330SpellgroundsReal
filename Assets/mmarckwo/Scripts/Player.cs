using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float maxHealth = 10.0f;
    [Header("Do not change this")]
    public float health;

    [Header("Physics")]    
    public float jumpForce = 10.0f;

    public float baseSpeed = 10.0f; 
    public float speed = 10.0f;
    private float translation;
    private float straffe;
    private Vector3 movement;
    private float smoother;
    
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
    public Transform respawnLocation;

    [Header("Health Bar Properties")]
    public GameObject healthBar;
    private Image healthBarFill;
    public Color goodHealth = new Color(69, 255, 137);
    public Color lowHealth = new Color(255, 0, 85);

    [Header("HP Sounds")]
    public AudioSource hpRestore;
    public AudioSource hpDrain;

    private Rigidbody rb;

    // score for the game, get to three to win.
    private int score = 0;
    private int enemyScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

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
        // gravity.
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);

        // movement.
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        movement = new Vector3(straffe, 0, translation);
        
        smoother = movement.magnitude;
        Mathf.Clamp(smoother, 0, 1);
        
        movement.Normalize();
        transform.Translate(movement * smoother);

        // check for jump availability.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void Update()
    {
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
        }
    }

    void Jump()
    {
        // play jump sound;
        jumpSound.Play();
        // player cannot cancel impulse attack by using addforce. (good thing)
        rb.AddForce(new Vector3(0, 1, 0) * jumpForce, ForceMode.Impulse);
        //rb.velocity = (new Vector3(0, 1, 0) * jumpForce);
    }

    // reset player stats and position after falling or dying.
    void Respawn()
    {
        gameObject.transform.position = respawnLocation.transform.position;
        health = maxHealth;
        speed = baseSpeed;
        HealthUpdate();
        rb.velocity = new Vector3(0, globalGravity, 0);

        if (gameObject.tag == "Enemy")
        {
            Debug.Log("enemy perished");
            score += 1;
            Debug.Log("Player score: " + score);
        }
        else if (gameObject.tag == "Player")
        {
            Debug.Log("player perished");
            enemyScore += 1;
            Debug.Log("Enemy score: " + enemyScore);
        }
    }

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
        }

        // update health bar fill amount.
        healthBarFill.fillAmount = health / maxHealth;
    }

    void HealthUp()
    {
        hpRestore.Play();

        // restore HP by some randomly decided value. 
        health += 40.2f;
        HealthUpdate();

    }

    void HealthDown()
    {
        hpDrain.Play();

        // decrease health by arbitrarily decided value.
        health -= 15.2f;
        HealthUpdate();
    }
}
