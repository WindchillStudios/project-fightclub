using UnityEngine;
using System.Collections;

public class Crusher : MonoBehaviour {

	Vector3 startPos;
	bool direction;

	// Use this for initialization
	void Start () {
		startPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		/*if (this.transform.position.x > startPos.x + 6.0f) {
			direction = true;
		}
		else if(this.transform.position.x < startPos.x - 6.0f){
			direction = false;
		}

		if(direction)
			this.gameObject.transform.Translate(Vector3.left/10);
		if(!direction)
			this.gameObject.transform.Translate(Vector3.right/10);*/
	}
}
