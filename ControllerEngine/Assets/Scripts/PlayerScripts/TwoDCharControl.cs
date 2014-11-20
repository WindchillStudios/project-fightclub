using UnityEngine;
using System.Collections;

public class TwoDCharControl : MonoBehaviour {

	public CharacterController body;
	public Health healthScript;

	public int playerNumber;

	bool onGround;

	//Knockback
	bool isKnocked;
	int knockTimer;
	Vector3 knockForce;

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
	public int facing;
	float horizontalMove;

	//Vertical
	public float jumpHeight;
	float currentJump;
	float jumpAcceleration;

	//Depth
	float depthCorrection;
	
	public float maxGravity;
	float gravity;
	float verticalMove;
	bool canJump = true;
	bool isJumping = false;
	bool isFalling = false;
	bool onRSlope = false;
	bool onLSlope = false;
	bool isDead = false;

	Vector3 movement = new Vector3 (0,0,0);
	
	// Use this for initialization
	void Start () {
		gravity = maxGravity;
		canDash = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Debug.Log(isFalling);

		isDead = healthScript.isDead;

		if(!isDead)
		{
			getMovement ();

			movement.x = horizontalMove * Time.deltaTime;
			movement.y = verticalMove * Time.deltaTime;
			movement.z = depthCorrection * Time.deltaTime;

			body.Move (movement);
		}

		if(isDead)
		{
			isKnocked = false;
			movement = Vector3.zero;
		}
		
	}

	void getMovement(){

		if(body.transform.position.z > 0)
		{
			depthCorrection = -0.1f;
		}
		else if(body.transform.position.z < 0)
		{
			depthCorrection = 0.1f;
		}

		// HORIZONTAL ///

		direction = Input.GetAxis ("Horizontal" + playerNumber) ;
		if(direction != 0)
		{
			if(direction > 0){
				facing = 1;
				body.transform.eulerAngles = new Vector3 (0, 90, 0);
			}
			else if(direction < 0){
				facing = -1;
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

		// KNOCKBACK ///

		if(isKnocked)
		{
			if(knockForce != Vector3.zero)
			{
				body.Move(knockForce);

				if(knockForce.y > 0)
				{
					knockForce.y -= 0.1f;
				}
				else
					isKnocked = false;
			}
			else{
				isKnocked = false;
				knockForce = new Vector3(0,0,0);
			}
			
		}

		// DASHING ///
		if(Input.GetAxis("Dash"  + playerNumber) > 0)
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
				Vector3 dashMove = new Vector3(facing,0,0);
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

		// VERTICAL ///

		if (Input.GetAxis ("Jump"  + playerNumber) > 0) {
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

	//SOME COLLISION DETECTION//

	void OnControllerColliderHit(ControllerColliderHit hit){


		// PHYSICS OBJECT INTERACTION ////////

		if(hit.gameObject.rigidbody)
		{
			hit.rigidbody.AddForceAtPosition(hit.moveDirection*10.0f, hit.point);
		}

		// JUMP CANCEL COLLISION ////////////

		if (hit.moveDirection.y == 1.0f) {
			isFalling = true;
		}

		// INSTANT DEATH OBJECT ////////////

		if (hit.gameObject.tag == "Death") { 
			healthScript.takeDamage(1000.0f,hit.gameObject);
		}

		// TRAP INTERACTION ///////////////

		if(hit.gameObject.tag == "Trap")
		{
			if(hit.gameObject.GetComponent<TrapScript>())
			{
				TrapScript trap = hit.gameObject.GetComponent<TrapScript>();
				if(trap.isActive == true)
				{
					trap.doTrap(this.gameObject);
				}
			}
		}

		// SLOPE CORRECTION ////////////////

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

	public void knockBack(Vector3 force)
	{
		isKnocked = true;
		knockForce = force;
	}
}
