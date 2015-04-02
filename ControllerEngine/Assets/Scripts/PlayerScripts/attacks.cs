using UnityEngine;
using System.Collections;

public class attacks : MonoBehaviour {

	Character character;
	public Vector2 attForce;
	public float currentDamage;
	Vector2 useForce;
	float useDamage;

	// Use this for initialization
	void Start () {
		character = this.GetComponentInParent<Character> ();
	}

	public void GetCurrentAttack(Vector2 force, float damage)
	{
		attForce = force;
		currentDamage = damage;
	}

	void OnTriggerEnter(Collider hit)
	{
		//FOR CHARACTER INTERACTION...////
		if(hit.gameObject.tag == "Player" && hit.gameObject.GetComponent<Character>())
		{
			if(hit.gameObject.GetComponent<Character>().playerNumber != character.playerNumber)
			{		
				hit.gameObject.GetComponent<Character>().knockBack(attForce);
				hit.gameObject.GetComponent<Character>().takeDamage(currentDamage, this.gameObject);
			}
		}

		//FOR PHYSICS INTERACTION...////

		if(hit.gameObject.GetComponent<Rigidbody>() && hit.gameObject.tag != "attackBox")
		{
			hit.gameObject.rigidbody.AddForce(new Vector3(character.facing,1,0)*50);
		}

		//FOR BREAKABLE LEVEL OBJECTS...////
	
		if(hit.gameObject.GetComponent<Breakable>())
		{
			hit.gameObject.GetComponent<Breakable>().breakObject();
		}

		//FOR SWITCHES...///

		if(hit.gameObject.tag == "Switch")
		{
			if(!hit.gameObject.GetComponent<SwitchScript>().isOn)
			{
				hit.gameObject.GetComponent<SwitchScript>().activate();
			}
			else if(hit.gameObject.GetComponent<SwitchScript>().isOn)
			{
				hit.gameObject.GetComponent<SwitchScript>().deactivate();
			}
		}
	}
}