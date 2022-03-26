using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseCamLook : MonoBehaviourPun
{
    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    // the chacter is the capsule
    public GameObject character;
    // get the incremental value of mouse moving
    private Vector2 mouseLook;
    // smooth the mouse moving
    private Vector2 smoothV;

    // reference variable for player script to access camera pitch.
    public Quaternion lookAngle;

    // Use this for initialization
    void Start()
    {

        this.GetComponent<Camera>().enabled = true;
        if (!this.photonView.IsMine)
        {
            this.GetComponent<Camera>().enabled = false;
        }
        character = this.transform.parent.gameObject;
    }

    // LateUpdate so that player's movement updates before the camera's movement.
    void LateUpdate()
    {
        // md is mouse delta
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        // the interpolated float result between the two float values
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);

        
        // incrementally add to the camera look
        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

        // vector3.right means the x-axis
        lookAngle = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);

        transform.localRotation = lookAngle;

        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
