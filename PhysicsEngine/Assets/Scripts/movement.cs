using UnityEngine;
using System.Collections;

public class movement : MonoBehaviour {
	
	GameObject player;
	public Rigidbody body;
	public CapsuleCollider bodyCollider;
	
	//Moving
	public float maxSpeed;
	float direction;
	Vector3 targetVelocity = new Vector3(0,0,0);
	Vector3 deltaVelocity = new Vector3(0,0,0);
	
	//Booooooooooools
	bool isRayGrounded;
	bool isColliderGrounded;
	bool onSlope;

	//Jumping
	public float jumpHeight;
	bool canJump = true;
	bool isJumping = false;
	Vector3 jumpVelocity = new Vector3(0,0,0);
	
	// Use this for initialization
	void Start () {
		player = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {

		spatialRays ();
		direction = Input.GetAxis("Horizontal");
		getMovement ();
		//getAnimations ();
		getJump ();

		if (isRayGrounded)
		{
			Debug.Log("OnGround");
			canJump = true;
			body.useGravity = false;
		}
		else
		{
			Debug.Log("NotOnGround");
			if(body.velocity.y < 0.1 || body.velocity.y > -0.1){
				body.AddForce (new Vector3(0,-1,0), ForceMode.Impulse);
			}
			canJump = false;
		}
	}
	
	void getAnimations(){
		if (direction != 0) {
			player.animation.Play("run");
		} else {
			player.animation.Play("idle");
		}
		if (isJumping) {
			player.animation.Play("walk");
		}
	}
	
	void getMovement() {
		
		if (direction != 0) {
			if(!onSlope)
			{
				if(direction < 0){
					targetVelocity.x = direction * maxSpeed;
					deltaVelocity = targetVelocity - rigidbody.velocity;
				}

				else if(direction > 0){
					targetVelocity.x = direction * maxSpeed;
					deltaVelocity = targetVelocity - rigidbody.velocity;
				}
			}				
			else
			{
				targetVelocity.x = direction * - 2;
				deltaVelocity = targetVelocity - rigidbody.velocity;
			}
			
		} else {
			deltaVelocity.x = 0;
		}
		if (rigidbody.useGravity) {
			deltaVelocity.y = 0;
			deltaVelocity.z = 0;
		}
		
		if (direction > 0) {
			player.transform.eulerAngles = new Vector3 (0, 90, 0);
		} else if (direction < 0) {
			player.transform.eulerAngles = new Vector3 (0, 270, 0);
		}
		
		body.AddForce (deltaVelocity, ForceMode.Acceleration);
	}
	
	void getJump(){
		
		if (canJump == true) {
			if (Input.GetAxis ("Jump") > 0) {
				body.velocity = new Vector3(0,0,0);
				isJumping = true;
			}
		}
		if(isJumping){
			canJump = false;
			
			jumpVelocity.y = jumpHeight;
			body.AddForce (jumpVelocity, ForceMode.Impulse);
			
			isJumping = false;
		}
	}
	
	void spatialRays(){

		isRayGrounded = false;

		RaycastHit hit;
		Vector3 down = body.transform.TransformDirection(Vector3.down);
		Ray footRay = new Ray (transform.position, down);

		Vector3 forward = body.transform.TransformDirection(Vector3.forward);
		Ray forRay = new Ray (transform.position + transform.TransformDirection(Vector3.down), forward);
		Ray forRay2 = new Ray (transform.position + transform.TransformDirection(Vector3.up), forward);

		Debug.DrawRay (transform.position, down * 1.2f);
		Debug.DrawRay (transform.position + transform.TransformDirection(Vector3.down), forward*0.5f);
		Debug.DrawRay (transform.position + transform.TransformDirection(Vector3.up), forward*0.5f);

		if (Physics.Raycast (footRay, out hit, 1.2f)) {
			
			if(hit.collider.tag == "Level")
			{
				isRayGrounded = true;
			}
		}

		if (Physics.Raycast (forRay, out hit, 0.5f) || Physics.Raycast (forRay2, out hit, 0.5f)) {

			float slope = (hit.normal.y/hit.normal.x);

			if(slope < 1 && slope > -1)
			{
				onSlope = true;
			}
		}
		else
			onSlope = false;
	}
}