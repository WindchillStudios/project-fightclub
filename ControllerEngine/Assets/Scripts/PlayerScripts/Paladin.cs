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
	public bool isDoSpecial;
	bool isHeld;

	new void Start () {
		
		maxJump = 25;
		maxSpeed = 15;
		maxHealth = 110;
		chargeTime = 0.4f;
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
				currentDamage = 12.0f;
				attForce = new Vector2(facing*5f,5f);
				
				break;
				
			case 1://No Input
				attackNum = 1;
				currentDamage = 12.0f;
				attForce = new Vector2(facing*1f,0);
				
				break;
				
			case 2://Up Input
				attackNum = 1;
				currentDamage = 12.0f;
				attForce = new Vector2(0,-1f);
				
				break;
				
			case 3://Down Input
				attackNum = 1;
				currentDamage = 12.0f;
				attForce = new Vector2(facing*1f,0);
				
				break;
				
			case 4:
				model.SetTrigger("isCombo");
				model.SetInteger("attackState", 0);
				
				currentDamage = 12.0f;
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


		if (Input.GetAxis ("SpecialAttack" + playerNumber) > 0 || actionInput == "Spcl") {
			isHeld = true;
		}
		else{
			isHeld = false;
		}

		/* ----- Do Attack ----- */

		if(model.GetCurrentAnimatorStateInfo(0).IsName("Special")){
			isDoSpecial = true;
			extMovement(new Vector3(facing*10,0,0));
		}
		else{
			isDoSpecial = false;
		}
		
		/* ----- Particle Glow ----- */

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

		/* ----- Charge Work ----- */

		if(chargeTimer > chargeTime){
			isCharged = true;
			chargeTimer = 0;
			charging = false;
			model.SetTrigger("charging");
		}

		if(isCharged && !isHeld && !canSpecial)
		{
			canSpecial = true;
		}

		if (isCharged && isHeld && !canSpecial) {
			model.SetInteger("attackState", 0);
		}

		if(!isDoSpecial){
			if(!isCharged)
			{
				if(charging){
						if(isHeld){
							chargeTimer += 0.1f * Time.deltaTime;
							model.SetInteger("attackState", 5);
						}
						else{
							charging = false;
							model.SetTrigger("charging");
						}
				}
				else{
					model.SetTrigger("charging");
					attackEffects.Stop();
				}
			}
			else{
				model.SetTrigger("charging");
				charging = false;
			}
		}
	}

	void DoSpecial()
	{
		if(!isDoSpecial){
			if(isCharged)
			{	
				if(canSpecial)
				{	
					currentDamage = 30.0f;
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
	}

	void DoBasic()
	{	
		charAttacks.GetCurrentAttack (attForce, currentDamage);
		model.SetInteger ("attackState", attackNum);
	}
}
