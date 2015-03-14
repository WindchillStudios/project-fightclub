using UnityEngine;
using System.Collections;

public class AsteroidRotation : MonoBehaviour {

	public float rotation;

	// Update is called once per frame
	void Update () {
		this.transform.Rotate (0,rotation,0);
	}
}
