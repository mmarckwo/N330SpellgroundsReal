using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MouseCamLook playerCam;

    public GameObject attackSpell;
    public float shootSpeed = 700f;

    public float maxHealth = 10.0f;
    private float health;

    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;
    bool isGrounded;

    public float speed = 10.0f;
    private float translation;
    private float straffe;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // set player to be at full HP.
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            // rotation combines player rotation and camera pitch.
            GameObject attack = Instantiate(attackSpell, transform.position, (transform.rotation * playerCam.lookAngle));
            attack.GetComponent<Rigidbody>().AddRelativeForce(0, 0, shootSpeed);

        }

        // temp. 
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        // add a sphere to the player to check for jump availability.
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask

        if (Input.GetKeyDown("escape"))
        {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
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

        Debug.Log(health);
    }
}
