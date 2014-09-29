using UnityEngine;
using System.Collections;

public class TwoDCharControl : MonoBehaviour {

	public CharacterController body;

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

	Vector3 movement = new Vector3 (0,0,0);


	// Use this for initialization
	void Start () {
		gravity = maxGravity;
	
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

		// VERTICAL ///

		if (Input.GetAxis ("Jump") > 0) {
			if(canJump){
				isJumping = true;
			}
		}
		if(body.isGrounded){
			canJump = true;
			jumpAcceleration = jumpHeight;
			Debug.Log("grouded");
			gravity = 0.0f;
		}

		if(isJumping)
		{
			Debug.Log(currentJump);
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
			if(gravity > maxGravity){
				gravity -= 2.0f;
			}
			verticalMove = gravity;
		}

	}
}
