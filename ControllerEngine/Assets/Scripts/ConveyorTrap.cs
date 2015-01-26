using UnityEngine;
using System.Collections;

public class ConveyorTrap : TrapScript {

	public float direction = 0.01f;
	public ParticleSystem rocks;
	Animator anim;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
		rocks.enableEmission = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(anim)
		{
			if(isActive)
			{
				anim.SetBool("isOn",true);
				rocks.enableEmission = true;
			}
			else if(!isActive)
			{
				anim.SetBool("isOn",false);
				rocks.enableEmission = false;
			}
		}
	}

	public override void doTrap(GameObject player){
		player.GetComponent<TwoDCharControl> ().knockBack(new Vector3(direction,0,0));
	}
}
