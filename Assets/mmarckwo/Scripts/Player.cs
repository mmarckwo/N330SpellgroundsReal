using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [Header("Physics")]
    public float maxHealth = 10.0f;
    private float health;
    public float jumpForce = 10.0f;
    
    public float speed = 10.0f;
    private float translation;
    private float straffe;
    private Vector3 movement;
    private float smoother;

    private Rigidbody rb;
    
    // player will use its own gravity for jump physics.
    public float gravityScale = 1.0f;
    public static float globalGravity = -9.81f;

    [Header("Jump Checking")]
    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Respawner")]
    public Transform respawnLocation;

    [Header("Health Bar Properties")]
    public GameObject healthBar;
    private Image healthBarFill;
    public Color goodHealth = new Color(69, 255, 137);
    public Color lowHealth = new Color(255, 0, 85);


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
            //rb.AddForce(new Vector3(0, 1, 0) * jumpForce, ForceMode.Impulse); 
            rb.velocity = (new Vector3(0, 1, 0) * jumpForce);
        }

        // respawn the player after going below 80 in the world.
        if(gameObject.transform.position.y <= -80)
        {
            gameObject.transform.position = respawnLocation.transform.position;
        }
    }

    void HealthUp()
    {
        // restore HP by some randomly decided value. 
        health += 4.2f;

        // clamp health to not go above max HP. 
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        // make health bar green again if player recovers enough HP.
        if (health / maxHealth > .30)
        {
            healthBarFill.color = goodHealth;
        }

        // update health bar fill amount.
        healthBarFill.fillAmount = health / maxHealth;

        Debug.Log(health);
    }

    void HealthDown()
    {
        // decrease health by arbitrarily decided value.
        health -= 1.2f;

        // clamp health to not go below 0. 
        if (health < 0)
        {
            health = 0;
        }

        // update health bar fill amount.
        healthBarFill.fillAmount = health / maxHealth;

        // make the health bar red when the player is at low HP.
        if ((health / maxHealth <= .30) || (health == 1))
        {
            healthBarFill.color = lowHealth;
        }
        // when the player dies.
        if (health == 0)
        {
            Debug.Log("DEAD");
        }

        Debug.Log(health);
    }
}
