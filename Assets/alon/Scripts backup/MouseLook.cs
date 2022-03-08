using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviourPun
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;
    public Camera myCamera;

    void Start()
    {
        if (!this.photonView.IsMine) return;
        this.GetComponent<Camera>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void TurnOnCamera()
    {
        if (!this.photonView.IsMine) return;
        this.GetComponent<Camera>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!this.photonView.IsMine) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
