﻿using UnityEngine;
using System.Collections;

public class Paladin : Character {
	
	float currentDamage;
	Vector2 attForce;
	int type;
	int attackNum;

	/* --- Paladin Specific --- */

 	float chargeTimer;
	float chargeTime;
	bool isCharged;
	bool charging;
	bool canSpecial;
	bool isSpecial;

	void Start () {
		
		maxJump = 35;
		maxSpeed = 15;
		maxHealth = 110;
		chargeTime = 0.5f;

		base.Start ();
	}

	public override void DoAttack(bool isSpecial, int attDirection){
			
		if (isSpecial)
		{
			DoSpecial();
		}
		else
		{
			switch(attDirection)
			{
			case 0://No Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(facing*10,3);
				break;
				
			case 1://No Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(facing*1f,0);
				break;
				
			case 2://Up Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(0,-1f);
				break;
				
			case 3://Down Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(facing*1f,0);
				break;
			
			case 4:
				attackNum = 1;
				currentDamage = 10.0f;
				attForce = new Vector2(1,1);
				break;

			default:
				break;
			}
			DoBasic();
		}
	}

	public override void updateSpecial(){
		if(isSpecial)
		{
			extMovement(new Vector3(facing*10,0,0));

			if(model.IsInTransition(0)){
				if(!model.GetNextAnimatorStateInfo(0).IsTag("Attack"))
				{
					isSpecial = false;
				}
			}
		}

		if(chargeTimer > chargeTime){
			isCharged = true;
			chargeTimer = 0;
			charging = false;
			model.SetTrigger("charging");
		}

		if(isMobileControlled){
			if(isCharged && (actionInput != "Spcl") && !canSpecial)
			{
				state_ = State.STATE_IDLE;
				canSpecial = true;
			}
		}
		else{
			if(isCharged && !(Input.GetAxis("SpecialAttack" + playerNumber) > 0) && !canSpecial)
			{
				state_ = State.STATE_IDLE;
				canSpecial = true;
			}
		}

		if(!isCharged)
		{
			if(charging){
				if(isMobileControlled){
					if(actionInput == "Spcl"){
						chargeTimer += 0.1f * Time.deltaTime;
						model.SetInteger("attackState", 3);
					}
					else{
						charging = false;
						model.SetTrigger("charging");
					}
				}
				else{
					if(Input.GetAxis("SpecialAttack" + playerNumber) > 0){
						chargeTimer += 0.1f * Time.deltaTime;
						model.SetInteger("attackState", 3);
					}
					else{
						charging = false;
						model.SetTrigger("charging");
					}
				}
			}
		}
		else
			charging = false;
	}

	void DoSpecial()
	{
		if(isCharged)
		{	
			if(canSpecial)
			{	
				isSpecial = true;
				currentDamage = 20.0f;
				attForce =  new Vector2 (facing*5,5);
				charAttacks.GetCurrentAttack (attForce, currentDamage);
			
				model.SetInteger("attackState", 4);
				canSpecial = false;
				isCharged = false;
			}
		}
		else if(!isCharged)
		{
			if(!charging)
			{
				model.SetInteger("attackState", 2);
				charging = true;
			}
		}
	}

	void DoBasic()
	{		
		charAttacks.GetCurrentAttack (attForce, currentDamage);
		model.SetInteger ("attackState", attackNum);
	}
}
