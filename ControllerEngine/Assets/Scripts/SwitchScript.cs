using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour {

	public GameObject mechanism;
	public bool isOn = false;
	bool canSwitch;
	Animator anim;

	void Start()
	{
		anim = this.GetComponent<Animator> ();
	}

	void switchable(){
		canSwitch = true;
	}

	// Use this for initialization
	public void activate () {
		if(anim)
		{
			if(mechanism.GetComponent<TrapScript>())
			{
				if(!mechanism.GetComponent<TrapScript>().isActive)
				{
					if(canSwitch){
						canSwitch = false;
						isOn = true;
						anim.SetBool("isOn",true);
						mechanism.GetComponent<TrapScript>().activate();
					}
				}
			}
		}
	}
	public void deactivate()
	{
		if(mechanism.GetComponent<TrapScript>())
		{
			if(mechanism.GetComponent<TrapScript>().isActive)
			{
				if(canSwitch){
					canSwitch = false;
					isOn = false;
					anim.SetBool("isOn",false);
					mechanism.GetComponent<TrapScript>().deactivate();
				}
			}
		}
	}
}
