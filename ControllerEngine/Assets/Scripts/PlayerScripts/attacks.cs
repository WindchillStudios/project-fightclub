using UnityEngine;
using System.Collections;

public class attacks : MonoBehaviour {

	public TwoDCharControl character;
	Animator anim;

	float currentDamage;
	Vector2 attForce;
	int type;

	int direction;

	bool canAttack;
	public bool isAttacking;


	// Use this for initialization
	void Start () {
		canAttack = true;
		anim = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		if(anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
		{		
			isAttacking = false;
			canAttack = true;
			anim.SetBool("isAttack", false );
			anim.SetBool("isSpecialAttack", false );
		}

		if (Input.GetAxis ("Horizontal" + character.playerNumber) != 0)
		{
			direction = 1;
		}
		else if (Input.GetAxis ("Vertical" + character.playerNumber) > 0)
		{
			direction = 2;
		}
		else if (Input.GetAxis ("Vertical" + character.playerNumber) < 0)
		{
			direction = 3;
		}
		else
			direction = 0;

		if(canAttack)
		{
			if(Input.GetAxis("Attack" + character.playerNumber) > 0)
			{
				DoAttack(false, direction);
			}
			else if(Input.GetAxis("SpecialAttack" + character.playerNumber) > 0)
			{
				DoAttack(true, direction);
			}
		}
	}

	void OnTriggerEnter(Collider hit)
	{
		//FOR CHARACTER INTERACTION...////

		if(hit.gameObject.tag == "Player" && hit.gameObject.GetComponent<TwoDCharControl>().playerNumber != character.playerNumber)
		{
			hit.gameObject.GetComponent<TwoDCharControl>().knockBack(attForce);
			hit.gameObject.GetComponent<Health>().takeDamage(currentDamage, this.gameObject);
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
			hit.gameObject.GetComponent<SwitchScript>().activate();
		}
	}

	void DoAttack(bool isSpecial, int attDirection){

		canAttack = false;
		isAttacking = true;

		if (isSpecial)
		{
			anim.SetBool ("isSpecialAttack", true);
			currentDamage = 20.0f;
		}
		else
		{
			switch(attDirection)
			{
				case 0://No Input
					anim.SetBool("isAttack", true );
					anim.SetInteger ("animDirection", 0);
					currentDamage = 10.0f;
					attForce = new Vector3(character.facing*1f,0);
					break;
				case 1://Forward Input
					anim.SetBool("isAttack", true );
					anim.SetInteger ("animDirection", 1);
					currentDamage = 10.0f;
					attForce = new Vector3(character.facing*1f,1f);
					break;
				case 2://Up Input
					anim.SetBool("isAttack", true );
					anim.SetInteger ("animDirection", 2);
					currentDamage = 10.0f;
					attForce = new Vector3(0,-1f);
					break;
				case 3://Down Input
					anim.SetBool("isAttack", true );
					anim.SetInteger ("animDirection", 3);
					currentDamage = 10.0f;
					attForce = new Vector3(character.facing*1f,0);
					break;
				default:
					break;
			}
		}
	}
}
