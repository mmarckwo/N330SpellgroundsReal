using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPun
{
    private CharacterController controller;
    public float speed = 5;
    public float gravity = -9.8f;

    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;
    bool isMonster = false;

    public bool stunned;
    private float stunReturn;
    private AudioSource killSound;
    public GameObject[] potentialSprites;
    private GameObject deathPoint;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        controller = this.GetComponent<CharacterController>();
        killSound = this.GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        if (this.photonView.IsMine)
        {
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["monster"])
            {
                isMonster = true;
                this.photonView.RPC("ActivateMonster", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void ActivateMonster()
    {
        this.GetComponent<MonsterSkills>().enabled = true;
        this.GetComponent<PlayerSkills>().enabled = false;
        this.potentialSprites[0].SetActive(false);
        this.potentialSprites[1].SetActive(true);
    }

    //combat

    public void AttackHit()
    {
        killSound.Play();
        this.photonView.RPC("Death", RpcTarget.All);
        GameObject GM = GameObject.Find("GameController");
        GM.SendMessage("DeathCount");
    }

    [PunRPC]
    void Death()
    {
        deathPoint = GameObject.Find("DeathPoint");
        this.gameObject.transform.position = deathPoint.transform.position;
    }

    //stun

    public void StunHit()
    {
        if (isMonster == true)
        {
            this.photonView.RPC("Stun", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Stun()
    {
        this.stunned = true;
        this.stunReturn = Time.time + 5f;
    }

    [PunRPC]
    public void UnStun()
    {
        this.stunned = false;
    }

    //movement

    void Update()
    {
        if (!this.photonView.IsMine) return;

        if (this.stunReturn > Time.time)
        {
            return;
        }
        else
        {
            this.photonView.RPC("UnStun", RpcTarget.All);

        }


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity);
        
        if(isMonster == false)
        {
            speed = 5;

            if (Input.GetKey("left shift"))
            {
                speed = 8;
            }
            else
            {
                speed = 5;
            }
        }
        else if (isMonster == true)
        {
            speed = 6;

            if (Input.GetKey("left shift"))
            {
                speed = 8;
            }
            else
            {
                speed = 6;
            }
        }
    }
}
