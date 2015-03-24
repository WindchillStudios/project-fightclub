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
	
	void Start () {
		
		maxJump = 25;
		maxSpeed = 15;
		maxHealth = 110;
		
		canSpecial = true;
		
		base.Start ();
	}
	
	public override void DoAttack(bool isSpecial, int attDirection){
		
		if (isSpecial)
		{
			chargeDamage = 5.0f;
			attForce = Vector2.zero;

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
			canSpecial = false;
		}
		else if(!isHeld) {
			if(model.GetInteger("attackState") == 3){
				model.SetInteger("attackState", 4);
			}
			canSpecial = true;
		}
		Debug.Log ("isHeld " + isHeld);

		if(Input.GetAxis("SpecialAttack" + playerNumber) > 0){
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
