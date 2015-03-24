using UnityEngine;
using System.Collections;

public class Bandit : Character {
	
	float currentDamage;
	Vector2 attForce;
	int type;
	int attackNum;
	
	/* --- Bandit Specific --- */

	public bool isHeld;
	public bool canSpecial;
	public float chargeDamage;
	
	void Start () {
		
		maxJump = 27;
		maxSpeed = 17;
		maxHealth = 90;

		canSpecial = true;

		base.Start ();
	}
	
	public override void DoAttack(bool isSpecial, int attDirection){

		if (isSpecial)
		{
			chargeDamage = 10.0f;
			attForce = new Vector2(direction*5f,0);
			if(canSpecial)
			{
				DoSpecial();
			}
			else
				state_ = State.STATE_IDLE;
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
				Debug.Log(attackNum);

				break;

			default:
				break;
			}
			DoBasic();
		}
		
		charAttacks.GetCurrentAttack (attForce, currentDamage);
	}

	public override void updateSpecial(){

		if (isHeld) {
			chargeDamage += 10 * Time.deltaTime;
		}
		else if(!isHeld){
			canSpecial = true;
			charAttacks.GetCurrentAttack (attForce, chargeDamage);
			model.SetInteger("attackState",4);
		}

		if(model.GetCurrentAnimatorStateInfo(0).IsName("Special")){
			canSpecial = false;

			if(isMobileControlled){
				if(actionInput == "Spcl"){
					isHeld = true;
				}
				else{
					Debug.Log("1");
					isHeld = false;
				}
			}
			else{
				if(Input.GetAxis("SpecialAttack" + playerNumber) > 0){
					isHeld = true;
				}
				else{
					Debug.Log("2");
					isHeld = false;
				}
			}
		}
		else{
			Debug.Log("3");
			isHeld = false;
		}
	}

	void DoSpecial()
	{
		model.SetInteger("attackState", 3);
	}

	void DoBasic()
	{
		model.SetInteger ("attackState", attackNum);
	}
}
