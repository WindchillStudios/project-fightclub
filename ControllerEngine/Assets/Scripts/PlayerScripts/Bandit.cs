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
	
	new void Start () {
		
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

			attForce = new Vector2(facing*5f,0);
			if(canSpecial)
			{
				DoSpecial();
			}
			//else
				//state_ = State.STATE_IDLE;
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
		
		if(model.IsInTransition(0)){
			if (state_ != State.STATE_ATTACKING) {
				canSpecial = true;
				chargeDamage = 0.0f;
			}
		}
		
		if(Input.GetAxis("SpecialAttack" + playerNumber) > 0 || actionInput == "Spcl"){
			isHeld = true;
		}
		else{
			isHeld = false;
		}
		
		if (isHeld) {
			if(model.GetInteger("attackState") == 3){
				chargeDamage += 10 * Time.deltaTime;
			}
		}
		else if(!isHeld){
			charAttacks.GetCurrentAttack (attForce, chargeDamage);
			
			if(model.GetInteger("attackState") == 3){
				model.SetInteger("attackState",4);
				model.SetTrigger("specialFinish");
			}
		}
		
		if (model.GetCurrentAnimatorStateInfo (0).IsName ("Special")) {
			
			charAttacks.GetCurrentAttack (attForce, chargeDamage);
			
			if(model.IsInTransition(0)){
				
				model.SetInteger("attackState",4);
				model.SetTrigger("specialFinish");
			}
		}
		
		if(model.GetInteger("attackState") == 4){
			if(!model.GetBool("specialFinish")){
				canSpecial = true;
			}
		}
	}

	void DoSpecial()
	{
		model.SetInteger("attackState", 3);
		canSpecial = false;
	}

	void DoBasic()
	{
		model.SetInteger ("attackState", attackNum);
	}
}
