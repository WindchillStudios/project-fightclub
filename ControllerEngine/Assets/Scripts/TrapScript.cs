using UnityEngine;
using System.Collections;

public class TrapScript : MonoBehaviour {

	public bool isActive = false;
	public GameObject target;

	void Start(){
	}

	void Update(){
	}

	public void activate () {
		if(!isActive)
		{
			isActive = true;
		}
	}

	public void deactivate () {
		if(isActive)
		{
			isActive = false;
		}
	}

	public void activate(GameObject inTarget){
		if(!isActive)
		{
			target = inTarget;
			isActive = true;					
		}
	}
	
	public virtual void doTrap(){
	}
	
	public virtual void doTrap(GameObject player){
	}
}
