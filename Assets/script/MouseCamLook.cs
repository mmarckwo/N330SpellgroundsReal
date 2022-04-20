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
    // the character is the capsule
    public GameObject character;
    // get the incremental value of mouse moving
    private Vector2 mouseLook;
    // smooth the mouse moving
    private Vector2 smoothV;
	
	public float mouseFollow;

    // reference variable for player script to access camera pitch.
    //public Quaternion lookAngle;

    private Vector3 cameraOffset;
    public Transform cameraTarget;
    public Texture2D crosshairTexture;
	public Camera particleCamera;
	
    // Use this for initialization
    void Start()
    {

        this.GetComponent<Camera>().enabled = true;
        if (!this.photonView.IsMine)
        {
            this.GetComponent<Camera>().enabled = false;
            this.particleCamera.enabled = false;
        }
        cameraOffset = transform.position - cameraTarget.position;
        //character = this.transform.parent.gameObject;
        Cursor.SetCursor(crosshairTexture, Vector2.zero, CursorMode.Auto);
    }

    // LateUpdate so that player's movement updates before the camera's movement.
    void LateUpdate()
    {
        if (!this.photonView.IsMine) return;

        // set the position of the camera to the middle of the player.
        Vector3 cameraPosition = cameraTarget.position + cameraOffset;
        transform.position = cameraPosition;

        // find the position of the mouse pointer, relative to the player.
		Vector3 hitPoint = new Vector3(0.0f,0.0f,0.0f);
		
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
		float height = mouseRay.origin.y - character.transform.position.y;
		
		Vector2 offset = new Vector2(mouseRay.direction.x,mouseRay.direction.z);
		
		if(mouseRay.direction.y != 0.0f){
			
			offset /= -mouseRay.direction.y;
			offset *= height;
			
			hitPoint = new Vector3(offset.x + mouseRay.origin.x, character.transform.position.y, offset.y + mouseRay.origin.z);
			
		}
		
		//offset the camera position based on the mouse pointer position.
		
		Vector3 mouseOffset = hitPoint - cameraPosition;
		
		mouseOffset = new Vector3(mouseOffset.x * mouseFollow, 0.0f, mouseOffset.z * mouseFollow);
		
		cameraPosition += mouseOffset;
		hitPoint += mouseOffset;
        transform.position = cameraPosition;
		
		character.transform.LookAt(hitPoint);
		
    }
}
