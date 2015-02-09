using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
/************
 * THIS SCRIPT IS ONLY A TEST SCRIPT, USED TO CONFIRM THAT VALUES CAN BE SENT PROPERLY. ACTUAL CONTROLLER SCRIPT SHOULD BE USED INSTEAD
*************/	
	private float speed = 0.2f;
	public TCPclient writeScript; 

	void Update() {
		if (Input.GetKeyDown ("space")) {
			Debug.Log("Space down!");
			writeScript.prepareString ("blue"); //This is how we pass things to the network to send it back to the controllers.
		}
	}
	/*This handles player movement, using words sent via the controller to issue the movement commands.*/
	public void playerInput(string input){
		if (input == "Jump") {
			Debug.Log("We in dis script");
			transform.position += (transform.up*speed);
		}
		if (input == "Rght") {
			transform.position += (transform.right*speed);
		}
	}
}