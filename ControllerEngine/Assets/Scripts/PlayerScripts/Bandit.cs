using UnityEngine;
using System.Collections;

public class Bandit : Character {
	
	float currentDamage;
	Vector2 attForce;
	int type;
	int attackNum;
	
	/* --- Bandit Specific --- */

	bool isHeld;
	
	void Start () {
		
		maxJump = 25;
		maxSpeed = 17;
		maxHealth = 90;
		
		base.Start ();
	}
	
	public override void DoAttack(bool isSpecial, int attDirection){

		
		if (isSpecial)
		{
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
