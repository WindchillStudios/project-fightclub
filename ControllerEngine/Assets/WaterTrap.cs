using UnityEngine;
using System.Collections;

public class WaterTrap : TrapScript {
	
	float activeTimer;
	public float activeEnd;
	Animator water;

	void Start(){
		water = this.GetComponent<Animator> ();
		activeEnd = 20.0f;
		water.SetBool("isRising",false);
	}
	
	void Update(){
		
		if(isActive)
		{
			activeTimer += 1*Time.deltaTime;
			water.SetBool("isRising",true);
		}
		else
		{
			if(water.GetCurrentAnimatorStateInfo(0).IsName("WaterRisen")){
				water.SetBool("isRising",false);
			}
		}

		if (activeTimer > activeEnd) {
			activeTimer = 0;
			isActive = false;
		}
	}

}
