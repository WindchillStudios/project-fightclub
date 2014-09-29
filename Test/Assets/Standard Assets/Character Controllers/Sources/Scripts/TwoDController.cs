using UnityEngine;
using System.Collections;

public class TwoDController : MonoBehaviour {

	GameObject player;
	public Rigidbody body;
	public CapsuleCollider collider;

	//Animations
	public Animation idleAnimation;
	public Animation walkAnimation;
	public Animation jumpAnimation;

	//Moving
	public float maxSpeed;
	float direction;
	Vector3 targetVelocity = new Vector3(0,0,0);
	Vector3 deltaVelocity = new Vector3(0,0,0);

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
	void FixedUpdate () {
		direction = Input.GetAxis("Horizontal");
		getMovement ();
		getAnimations ();
		getJump ();
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
			targetVelocity.x = direction * maxSpeed;
			deltaVelocity = targetVelocity - rigidbody.velocity;
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

		body.AddForce (deltaVelocity*50, ForceMode.Acceleration);
	}

	void getJump(){
		Debug.Log(body.velocity.y);

		if(body.velocity.y < 0.1 || body.velocity.y > -0.1){
			body.AddForce (new Vector3(0,-1,0), ForceMode.Impulse);
		}

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
	void OnCollisionEnter(Collision collision) {
		canJump = true;
	}
}
