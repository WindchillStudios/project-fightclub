using UnityEngine;
using System.Collections;

public class TwoDCharControl : MonoBehaviour {

	public CharacterController body;

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
	bool onRSlope = false;
	bool onLSlope = false;

	Vector3 movement = new Vector3 (0,0,0);
	
	// Use this for initialization
	void Start () {
		gravity = maxGravity;
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

			if (currentSpeed < maxSpeed) {
				currentSpeed += acceleration;
			}
		}
		else{
			currentSpeed = 0;
		}

		if(!onRSlope && !onLSlope)
			horizontalMove = currentSpeed * direction;

		if(onRSlope)
			horizontalMove = -7.0f;
  		else if(onLSlope)
			horizontalMove = 7.0f;

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
			isFalling = false;
			canJump = true;
			jumpAcceleration = jumpHeight;
		}

		if(isJumping)
		{
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
	void OnControllerColliderHit(ControllerColliderHit hit){
		Debug.Log (hit.moveDirection);

		if (hit.moveDirection.y == 1.0f) {
			if(hit.collider.tag == "Level")
			{
				isFalling = true;
			}
		}

		if(hit.moveDirection.y < -0.5f)
		{
			if(hit != null)
			{
				float slope = (hit.normal.y/hit.normal.x);

				if(slope < 1 && slope > 0){
					onLSlope = true;
				}
				else{
					onLSlope = false;
				}

				if(slope < 0 && slope > -1){
					onRSlope = true;
				}
				else{
					onRSlope = false;
				}
			}
			else{
				onRSlope = false;
				onLSlope = false;
			}
		}
	}
		
}
