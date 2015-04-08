using UnityEngine;
using System.Collections;

public class Dragon : Character {
	
	float currentDamage;
	Vector2 attForce;
	int type;
	int attackNum;
	
	/* --- Dragon Specific --- */
	
	public bool isHeld;
	public bool canSpecial;
	public float chargeDamage;
	
	new void Start () {
		
		maxJump = 25;
		maxSpeed = 15;
		maxHealth = 110;
		
		canSpecial = true;
		
		base.Start ();
	}
	
	public override void DoAttack(bool isSpecial, int attDirection){
		
		if (isSpecial)
		{
			attForce = Vector2.zero;
			currentDamage = 1.0f;

			if(canSpecial)
			{
				DoSpecial();
			}
			else{
				state_ = State.STATE_IDLE;
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
	
	public override void updateSpecial(){

		if (isHeld) {
			canSpecial = false;
		}
		else if(!isHeld) {
			if(model.GetCurrentAnimatorStateInfo(0).IsName("Special Spray") || model.GetCurrentAnimatorStateInfo(0).IsName("Special Windup")){
				model.SetInteger("attackState", 4);
			}
			canSpecial = true;
		}

		if(Input.GetAxis("SpecialAttack" + playerNumber) > 0 || actionInput == "Spcl"){
			isHeld = true;
		}
		else{
			isHeld = false;
		}
	}
	
	void DoSpecial()
	{
		model.SetInteger("attackState", 3);
		model.SetTrigger ("isSpraying");
	}
	
	void DoBasic()
	{
		model.SetInteger ("attackState", attackNum);
	}
}
