using UnityEngine;
using System.Collections;

public class ConveyorTrap : TrapScript {

	public float direction = 0.01f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public override void doTrap(GameObject player){
		player.GetComponent<TwoDCharControl> ().knockBack(new Vector3(direction,0,0));
	}
}
