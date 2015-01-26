using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour {

	public GameObject mechanism;
	public bool isOn = false;
	Animator anim;

	void Start()
	{
		anim = this.GetComponent<Animator> ();
	}

	// Use this for initialization
	public void activate () {
		if(anim)
		{
			if(mechanism.GetComponent<TrapScript>())
			{
				if(!mechanism.GetComponent<TrapScript>().isActive)
				{
					isOn = true;
					anim.SetBool("isOn",true);
					mechanism.GetComponent<TrapScript>().activate();
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
				isOn = false;
				anim.SetBool("isOn",false);
				mechanism.GetComponent<TrapScript>().deactivate();
			}
		}
	}
}
