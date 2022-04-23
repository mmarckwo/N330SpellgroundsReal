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
	
	public float mouseFollow; //how much should the camera move to follow the mouse pointer?
	
	float inaccuracy; //accumulated variable of how "inaccurate" we are. Increased by x radians when the player rotates x radians.
	public float inaccuracyCooldown; //every second, we decrement inaccuracy by this much.
	public float inaccuracyMultiplier; //for every degree in the inaccuracy accumulator, how many radians (in either direction) could the player's shot miss by?
	public float inaccuracyMax; //maximum inaccuracy we can have in the inaccuracy accumulator.
	
    // reference variable for player script to access camera pitch.
    //public Quaternion lookAngle;

    private Vector3 cameraOffset;
    public Transform cameraTarget;
    public Texture2D crosshairTexture;
	public Camera particleCamera;
	
	public GameObject playerCone;
	private SpriteRenderer spriteRenderer; //sprite renderer of player con
	
	public PlayerAttack playerAttack; //reference to player attack script
	
	public float GetInaccurateAngle()
	{
		
		float radius = this.inaccuracy * this.inaccuracyMultiplier;
		
		/*if(Random.Range(0.0f,1.0f) <= 0.5f){
			
			return(radius);
			
		}else{
			
			return(-radius);
			
		}*/
		
		return(Random.Range(-radius,radius));
		
	}
	
	void setPlayerConeVisual()
	{
		
		const float angleBias = 0.07f;
		const float scaleFactor = 2.0f;
		
		//get angle
		float angle = this.inaccuracy * this.inaccuracyMultiplier + angleBias;
		
		float scale = Mathf.Tan(angle) * scaleFactor;
		
		playerCone.transform.localScale = new Vector3(scale*playerCone.transform.localScale.y,playerCone.transform.localScale.y,playerCone.transform.localScale.z);
		
		float ratio = playerAttack.chargeTimer/playerAttack.chargeSpeed;
		float colorRatio = 1.0f-0.5f*ratio;
		float alpha = (50.0f/255.0f) + 0.2f*ratio;
		
		spriteRenderer.color = new Color(1.0f,colorRatio,colorRatio,alpha);
		
	}
	
    // Use this for initialization
    void Start()
    {
		
        this.GetComponent<Camera>().enabled = true;
        
		if (!this.photonView.IsMine)
        {
            this.GetComponent<Camera>().enabled = false;
            this.particleCamera.enabled = false;
			this.playerCone.SetActive(false);
        }
        
		this.spriteRenderer = playerCone.GetComponent<SpriteRenderer>();
		
		cameraOffset = transform.position - cameraTarget.position;
        //character = this.transform.parent.gameObject;
        Cursor.SetCursor(crosshairTexture, Vector2.zero, CursorMode.Auto);
		
		inaccuracy = 0.0f;
    }

    // LateUpdate so that player's movement updates before the camera's movement.
    void LateUpdate()
    {
        if (!this.photonView.IsMine) return;
		
		inaccuracy = Mathf.Max(0.0f,inaccuracy-inaccuracyCooldown*Time.deltaTime);
		
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
		
		Vector3 previousDirection = character.transform.forward;
		
		character.transform.LookAt(hitPoint);
		
		Vector3 currentDirection = character.transform.forward;
		
		currentDirection.Normalize();
		previousDirection.Normalize();
		
		float directionDifference = Mathf.Acos(Vector3.Dot(currentDirection,previousDirection));
		
		if(!float.IsNaN(directionDifference)){
			
			//Debug.Log(currentDirection);
			//Debug.Log(previousDirection);
			
			directionDifference = Mathf.Abs(directionDifference);
			
			/*if(directionDifference >= 0.05f){
				
				Debug.Log(currentDirection);
				Debug.Log(previousDirection);
				Debug.Log(directionDifference);
				
			}*/
			
			inaccuracy = Mathf.Min(inaccuracy+directionDifference,inaccuracyMax);
			
		}
		
		setPlayerConeVisual();
		
    }
}
