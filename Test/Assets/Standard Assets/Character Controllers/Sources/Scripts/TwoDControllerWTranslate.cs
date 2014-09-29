using UnityEngine;
using System.Collections;

public class TwoDControllerWTranslate : MonoBehaviour {

	float speed = 5.0f; // units per second
	float turnSpeed = 90.0f; // degrees per second
	float jumpSpeed = 8.0f;
	float gravity = 9.8f;
	private float vSpeed = 0.0f; // current vertical velocity
	public CharacterController controller;

	void Update(){
		transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);
		Vector3 vel = transform.forward * Input.GetAxis("Vertical") * speed;
		if (controller.isGrounded){
			vSpeed = 0; // grounded character has vSpeed = 0...
			if (Input.GetKeyDown("space")){ // unless it jumps:
				vSpeed = jumpSpeed;
			}
		}
		// apply gravity acceleration to vertical speed:
		vSpeed -= gravity * Time.deltaTime;
		vel.y = vSpeed; // include vertical speed in vel
		// convert vel to displacement and Move the character:
		controller.Move(vel * Time.deltaTime);
	}

}