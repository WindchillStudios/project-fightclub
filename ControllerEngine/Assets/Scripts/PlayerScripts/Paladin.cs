using UnityEngine;
using System.Collections;

public class Paladin : Character {
	
	float currentDamage;
	Vector2 attForce;
	int type;
	int attackNum;

	/* --- Paladin Specific --- */

 	public float chargeTimer;
	ParticleSystem attackEffects;
	float chargeTime;
	public bool isCharged;
	public bool charging;
	public bool canSpecial;
	bool isSpecial;

	new void Start () {
		
		maxJump = 25;
		maxSpeed = 15;
		maxHealth = 110;
		chargeTime = 0.5f;
		attackEffects = GetComponentInChildren<ParticleSystem> ();

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

		if (charging || isCharged) {
			Debug.Log(attackEffects);
			if(!attackEffects.isPlaying){
				attackEffects.Play();
			}
		}
		else{
			if(attackEffects.isPlaying){
				attackEffects.Stop();
			}
		}

		if(chargeTimer > chargeTime){
			isCharged = true;
			chargeTimer = 0;
			charging = false;
			model.SetTrigger("charging");
		}

		if(isCharged && (actionInput != "Spcl") && !canSpecial)
		{
			state_ = State.STATE_IDLE;
			canSpecial = true;
		}
		else if(isCharged && !(Input.GetAxis("SpecialAttack" + playerNumber) > 0) && !canSpecial)
		{
			state_ = State.STATE_IDLE;
			canSpecial = true;
		}

		if(!isCharged)
		{
			if(charging){

				if(actionInput == "Spcl" || Input.GetAxis("SpecialAttack" + playerNumber) > 0){
					chargeTimer += 0.1f * Time.deltaTime;
					model.SetInteger("attackState", 5);
				}
				else{
					charging = false;
					model.SetTrigger("charging");
				}
			}
			else{
				attackEffects.Stop();
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
			
				model.SetInteger("attackState", 6);
				canSpecial = false;
				isCharged = false;
			}
		}
		else if(!isCharged)
		{
			if(!charging)
			{
				model.SetInteger("attackState", 4);
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
