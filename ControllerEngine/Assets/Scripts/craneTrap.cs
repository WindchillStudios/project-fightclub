using UnityEngine;
using System.Collections;

public class craneTrap : TrapScript {

	public GameObject[] mechanisms;
	float activeTimer;
	public float activeEnd;
	float restPos;
	bool conveyorOn;

	void Start(){
		activeEnd = 10.0f;
		restPos = this.transform.eulerAngles.y;
	}

	void Update(){

		if(isActive)
		{
			if(this.transform.eulerAngles.y > 1){
				this.transform.Rotate(new Vector3(0,-0.1f,0));
			}

			conveyorOn = true;
			foreach(GameObject mechanism in mechanisms)
			{
				mechanism.GetComponent<TrapScript>().activate();
			}

			if(activeTimer < activeEnd){
				activeTimer += 1 * Time.deltaTime;
			}
			else{
				activeTimer = 0;
				isActive = false;
			}
		}
		else
		{
			if(this.transform.eulerAngles.y < restPos){
				this.transform.Rotate(new Vector3(0,0.1f,0));
			}

			if(conveyorOn){
				foreach(GameObject mechanism in mechanisms)
				{
					mechanism.GetComponent<TrapScript>().deactivate();
				}
			}
			conveyorOn = false;
		}
	}
}
