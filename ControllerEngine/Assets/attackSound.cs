using UnityEngine;
using System.Collections;

public class attackSound : MonoBehaviour {
	
	public void getAttackSound(){
		GetComponentInParent<Character> ().doAttackSound ();
	}
}
