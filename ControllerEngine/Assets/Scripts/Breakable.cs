using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {

	public void breakObject(){
		this.gameObject.SetActive (false);
	}

}
