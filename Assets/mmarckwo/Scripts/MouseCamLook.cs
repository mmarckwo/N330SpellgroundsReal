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

    // reference variable for player script to access camera pitch.
    //public Quaternion lookAngle;

    private Vector3 cameraOffset;
    public Transform cameraTarget;
    public Texture2D crosshairTexture;

    // Use this for initialization
    void Start()
    {

        this.GetComponent<Camera>().enabled = true;
        if (!this.photonView.IsMine)
        {
            this.GetComponent<Camera>().enabled = false;
        }
        cameraOffset = transform.position - cameraTarget.position;
        //character = this.transform.parent.gameObject;
        Cursor.SetCursor(crosshairTexture, Vector2.zero, CursorMode.Auto);
    }

    // LateUpdate so that player's movement updates before the camera's movement.
    void LateUpdate()
    {
        // camera follows player.
        Vector3 cameraPosition = cameraTarget.position + cameraOffset;
        transform.position = cameraPosition;

        // POINT TOWARDS MOUSE CURSOR. 
        //mouseTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
		float height = mouseRay.origin.y - character.transform.position.y;
		
		Vector2 offset = new Vector2(mouseRay.direction.x,mouseRay.direction.z);
		
		if(mouseRay.direction.y != 0.0f){
			
			offset /= -mouseRay.direction.y;
			offset *= height;
			
			Vector3 hitPoint = new Vector3(offset.x + mouseRay.origin.x, character.transform.position.y, offset.y + mouseRay.origin.z);
			
			character.transform.LookAt(hitPoint);
			
		}
		
		//v2: 
		// I don't like raycasting on update but it's more accurate than the below code.
		
		/*Plane p = new Plane(Vector3.up, character.transform.position);
		
        if(p.Raycast(mouseRay, out float hitDist))
        {
            Vector3 hitPoint = mouseRay.GetPoint(hitDist);
            character.transform.LookAt(hitPoint);
        
		}*/
		
		
		//v1:
		

        // md is mouse delta
        //var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        //md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        // the interpolated float result between the two float values
        //smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        //smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);


        // incrementally add to the camera look
        //mouseLook += smoothV;
        //mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

        // vector3.right means the x-axis
        //lookAngle = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);

        //transform.localRotation = lookAngle;

        //character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);

        // Using some math to calculate the point of intersection between the line going through the camera and the mouse position with the XZ-Plane
        // (I found this elsewhere)
        //float t = Camera.main.transform.position.y / (Camera.main.transform.position.y - mouseTarget.y);
        //Vector3 finalPoint = new Vector3(t * (mouseTarget.x - Camera.main.transform.position.x) + Camera.main.transform.position.x, 1, t * (mouseTarget.z - Camera.main.transform.position.z) + Camera.main.transform.position.z);

        //character.transform.LookAt(finalPoint, Vector3.up);
        // reset player X and Z rotations.
        //character.transform.eulerAngles = new Vector3(0, character.transform.eulerAngles.y, 0);
    }
}
