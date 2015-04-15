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
	bool isShooting;

	new void Start () {
		
		maxJump = 26;
		maxSpeed = 16;
		maxHealth = 100;

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
				isShooting = false;
				canShoot = true;
				currentShot = 0;
			}
		}

		if(model.GetCurrentAnimatorStateInfo(0).IsName("Special Shoot")){
			if(!isShooting){
				FireBoulder();
			}
		}

		if(!model.GetBool("isShooting")){
			model.SetInteger("attackState", 0);
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
		model.SetInteger("attackState", 3);
		model.SetTrigger ("isShooting");
		canShoot = false;
	}

	void FireBoulder(){
		isShooting = true;
		GameObject newBoulder = Instantiate (boulder, handSpawner, this.transform.rotation) as GameObject;
		newBoulder.GetComponent<boulderScript> ().parentNumber = this.playerNumber;
		newBoulder.GetComponent<Rigidbody> ().AddForce (20 * facing, 0, 0, ForceMode.Impulse);
	}
	
	void DoBasic()
	{
		model.SetInteger ("attackState", attackNum);
	}
}
