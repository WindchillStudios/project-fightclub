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

	void Start () {
		
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
			attForce = new Vector2(direction*5f,0);

			if(canShoot){
				DoSpecial();
			}
		}
		else
		{
			switch(attDirection)
			{
			case 0://No Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(direction*1f,0);

				break;
				
			case 1://No Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(direction*1f,0);

				break;
				
			case 2://Up Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(0,-1f);

				break;
				
			case 3://Down Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(direction*1f,0);

				break;

			case 4:
				if(attackNum < 2){
					attackNum = attackNum + 1;
				}
				else{
					canListen = false;
				}

				currentDamage = 10.0f;
				attForce = new Vector2(1,1);
				break;
			
			default:
				break;
			}
			DoBasic();
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
