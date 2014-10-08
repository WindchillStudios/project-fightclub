using UnityEngine;
using System.Collections;

public class attacks : MonoBehaviour {

	BoxCollider hitBox;

	float currentDamage;
	float force;
	int type;

	int direction;

	bool canAttack;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetAxis ("horizontal") != 0)
			direction = 1;
		else if (Input.GetAxis ("vertical") > 0)
			direction = 2;
		else if (Input.GetAxis ("vertical") < 0)
			direction = 3;
		else
			direction = 0;

		if(canAttack)
		{
			if(Input.GetAxis("q") > 0)
			{
				DoAttack(false, direction);
			}

			if(Input.GetAxis("w") > 0)
			{
				DoAttack(true, direction);
			}
		}
	}

	void OnCollisionEnter(Collision collider)
	{
		if(collider.gameObject.tag == "Player")
		{
			collider.gameObject.GetComponent<Health>().takeDamage(currentDamage,this.gameObject);
		}

		///////FOR BREAKABLE LEVEL OBJECTS...////
	
		/*if(collider.gameObject.GetComponent<Breakable>())
		{
			collider.gameObject.GetComponent<Breakable>().break();
		}*/
	}

	void DoAttack(bool isSpecial, int attDirection){
		//this.animation.play ();\
	}
}
