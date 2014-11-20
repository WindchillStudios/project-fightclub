using UnityEngine;
using System.Collections;

public class TrapScript : MonoBehaviour {

	public bool isActive = false;

	void Update(){
	}

	public void activate () {
		if(!isActive)
		{
			isActive = true;
		}
	}

	public virtual void doTrap(GameObject player){
	}
}
