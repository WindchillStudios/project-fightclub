using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour {

	public GameObject mechanism;

	// Use this for initialization
	public void activate () {
		if(mechanism.GetComponent<TrapScript>())
		{
			mechanism.GetComponent<TrapScript>().activate();
		}
	}
}
