using UnityEngine;
using System.Collections;

public class Golem : Character {
	
	float currentDamage;
	Vector2 attForce;
	int type;
	int attackNum;

	/* --- Golem Specific --- */

	public GameObject boulder;
	public bool canShoot;
	public float shotDelay;
	public float currentShot;
	Vector3 handSpawner;

	new void Start () {
		
		maxJump = 25;
		maxSpeed = 15;
		maxHealth = 110;

		shotDelay = 6;
		canShoot = true;

		base.Start ();
	}

	public override void updateSpecial(){
		handSpawner = new Vector3 ((this.transform.position.x + facing*2),this.transform.position.y+3,this.transform.position.z);

		if(!canShoot){
			if(currentShot < shotDelay){
				currentShot += 1 * Time.deltaTime;
			}
			else{
				canShoot = true;
				currentShot = 0;
			}
		}
	}

	public override void DoAttack(bool isSpecial, int attDirection){
		

		if (isSpecial)
		{
			currentDamage = 20.0f;
			attForce = new Vector2(facing*5f,0);

			if(canShoot){
				DoSpecial();
			}
		}
		else
		{
			switch(attDirection)
			{
			case 0://No Input
				attackNum = 1;
				currentDamage = 10.0f;
				attForce = new Vector2(facing*5f,5f);
				
				break;
				
			case 1://No Input
				attackNum = 1;
				currentDamage = 10.0f;
				attForce = new Vector2(facing*1f,0);
				
				break;
				
			case 2://Up Input
				attackNum = 1;
				currentDamage = 10.0f;
				attForce = new Vector2(0,-1f);
				
				break;
				
			case 3://Down Input
				attackNum = 1;
				currentDamage = 10.0f;
				attForce = new Vector2(facing*1f,0);
				
				break;
				
			case 4:
				model.SetTrigger("isCombo");
				model.SetInteger("attackState", 0);
				
				currentDamage = 10.0f;
				if(model.GetCurrentAnimatorStateInfo(0).IsName("Attack 2")){
					attForce = new Vector2(facing*5f,5f);
				}
				else if(model.GetCurrentAnimatorStateInfo(0).IsName("Attack 3")){
					attForce = new Vector2(facing*15f,6f);
				}
				
				break;
				
			default:
				break;
			}
			if(!model.GetBool("isCombo")){
				DoBasic();
			}
		}
		
		charAttacks.GetCurrentAttack (attForce, currentDamage);
	}
	
	void DoSpecial()
	{
		//Debug.Log ("Special");
		GameObject newBoulder = Instantiate (boulder, handSpawner, this.transform.rotation) as GameObject;
		newBoulder.GetComponent<boulderScript> ().parentNumber = this.playerNumber;
		newBoulder.GetComponent<Rigidbody> ().AddForce (20 * facing, 0, 0, ForceMode.Impulse);
		model.SetInteger("attackState", 3);
		canShoot = false;
	}
	
	void DoBasic()
	{
		model.SetInteger ("attackState", attackNum);
	}
}
