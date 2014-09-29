using UnityEngine;
using System.Collections;

public class TwoDCharControl : MonoBehaviour {

	public CharacterController body;

	bool canMove;
	bool onGround;

	//Dashning
	bool canDash;
	bool isDashing;
	float dashTimer = 0.0f;
	float dashCool = 0.0f;

	//Horizontal
	public float maxSpeed;
	float currentSpeed = 0.0f;
	float acceleration = 5.0f;
	float direction;
	float horizontalMove;

	//Vertical
	public float jumpHeight;
	float currentJump;
	float jumpAcceleration;

	public float maxGravity;
	float gravity;
	float verticalMove;
	bool canJump = true;
	bool isJumping = false;
	bool isFalling = false;

	Vector3 movement = new Vector3 (0,0,0);
	
	// Use this for initialization
	void Start () {
		gravity = maxGravity;
		canMove = true;
		canDash = true;
	}
	
	// Update is called once per frame
	void Update () {

		getMovement ();

		movement.x = horizontalMove * Time.deltaTime;
		movement.y = verticalMove * Time.deltaTime;

		body.Move (movement);
		
	}
	void getMovement(){

		RaycastHit hit;
		Ray headRay = new Ray (transform.position, Vector3.up);

		Vector3 up = body.transform.TransformDirection(Vector3.up) * body.height;

		Debug.DrawRay (body.transform.position, up, Color.red);

		if (Physics.Raycast (headRay, out hit, body.height)) {
			if(hit.collider.tag == "ground")
				isFalling = true;
		}


		Ray footRay = new Ray (body.transform.position, Vector3.down);
		Vector3 forward = body.transform.TransformDirection(Vector3.down) * body.height*1.5f;
		Debug.DrawRay (body.transform.position, forward, Color.red);

		if (Physics.Raycast (footRay, out hit, body.height*1.5f)) {
			if(hit.collider.tag == "ground")
			{
				float slope = (hit.normal.y/hit.normal.x);

				if(slope < 1 && slope > 0){
					canMove = false;

					body.Move(new Vector3(0.2f,0,0));
				}
				else if(slope > -1 && slope < 0){
					canMove = false;
					
					body.Move(new Vector3(-0.2f,0,0));
				}
				else
					canMove = true;
			}
		}

		
		// HORIZONTAL ///

		direction = Input.GetAxis ("Horizontal");
		if(direction != 0)
		{
			if(direction > 0){
				body.transform.eulerAngles = new Vector3 (0, 90, 0);
			}
			else if(direction < 0){
				body.transform.eulerAngles = new Vector3 (0, 270, 0);
			}

			animation.Play("run");
			if (currentSpeed < maxSpeed) {
				currentSpeed += acceleration;
			}
		}
		else{
			currentSpeed = 0;
			animation.Play("idle");
		}

		horizontalMove = currentSpeed * direction;

		// DASHING ///
		if(direction != 0){

			if(Input.GetAxis("Dash") > 0)
			{			
				if(canDash)
				{
					dashCool = 0.0f;
					dashTimer = 0.0f;
					isDashing =true;
				}
			}

			if(isDashing){
				canDash = false;

				if(dashTimer < 3){
					dashTimer ++;
					Vector3 dashMove = new Vector3(1*direction,0,0);
					body.Move(dashMove*1.0f);
				}
			}

			if(!canDash){

				if(dashCool < 100)
				{
					dashCool ++;
					Debug.Log(dashCool);
				}
				else
				{
					canDash = true;
				}
			}
		}

		// VERTICAL ///

		if (Input.GetAxis ("Jump") > 0) {
			if(canJump){
				isJumping = true;
			}
		}

		if(body.isGrounded){
			canMove = true;
			isFalling = false;
			canJump = true;
			jumpAcceleration = jumpHeight;
		}

		if(isJumping)
		{
			animation.Play("jump_pose");
			canJump = false;
			gravity = 0.0f;

			currentJump = jumpAcceleration;
			if(jumpAcceleration > 0){
				jumpAcceleration -= 1.0f;

			}
			else 
			{
				jumpAcceleration = jumpHeight;
				currentJump = 0.0f;
				isJumping = false;
			}
			verticalMove = currentJump;
		}
		else{
			isFalling = true;
		}

		if (isFalling) {
			isJumping = false;
			
			if(gravity > maxGravity){
				gravity -= 2.0f;
			}
			verticalMove = gravity;
		}

	}
}
