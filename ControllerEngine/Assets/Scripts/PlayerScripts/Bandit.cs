using UnityEngine;
using System.Collections;

public class Bandit : Character {
	
	float currentDamage;
	Vector2 attForce;
	int type;
	int attackNum;
	
	/* --- Bandit Specific --- */
	
	
	void Start () {
		
		maxJump = 22;
		maxSpeed = 12;
		maxHealth = 90;
		
		base.Start ();
	}
	
	public override void DoAttack(bool isSpecial, int attDirection){
		
		float cancelStart = 0;
		float cancelEnd = 0;
		float cancelTimer = 0;
		
		cancelTimer += 1 * Time.deltaTime;
		
		if (isSpecial)
		{
			cancelStart = 2;
			cancelEnd = 15;
			attackNum = 2;
			currentDamage = 20.0f;
			attForce = new Vector2(direction*5f,0);
			DoSpecial();
		}
		else
		{
			switch(attDirection)
			{
			case 0://No Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(direction*1f,0);
				cancelStart = 1f;
				cancelEnd = 3f;
				break;
				
			case 1://No Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(direction*1f,0);
				cancelStart = 1f;
				cancelEnd = 3f;
				break;
				
			case 2://Up Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(0,-1f);
				cancelStart = 0.1f * Time.deltaTime;
				cancelEnd = 0.4f * Time.deltaTime;
				break;
				
			case 3://Down Input
				attackNum = 0;
				currentDamage = 10.0f;
				attForce = new Vector2(direction*1f,0);
				cancelStart = 0.1f * Time.deltaTime;
				cancelEnd = 0.4f * Time.deltaTime;
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
		model.SetInteger("attackState", 2);
	}
	
	void DoBasic()
	{
		model.SetInteger ("attackState", attackNum);
	}
}
