using UnityEngine;
using System.Collections;

public class HazardSpawner : TrapScript {

	public float hazFreq;
	public int hazDur;
	public int hazWidth;
	public Rigidbody hazard;

	float hazardTimer;
	float spawnTimer;
	float rangeMax;

	public bool isRandom;

	void Start(){
		rangeMax = this.transform.position.x + hazWidth;
	}

	void Update(){
		if (isActive) {
			target = GameObject.FindGameObjectWithTag ("Player");

			hazardTimer += 1 * Time.deltaTime;
			spawnTimer += 1 *Time.deltaTime;

			if(hazFreq > 0)
			{
				if (hazardTimer > hazDur)
				{
					isActive = false;
				}

				if (spawnTimer > hazFreq) {

					if(isRandom)
						doTrap ();
					else
						doTrap(target);

					spawnTimer = 0;
				}
			}
			else
			{
				if(isRandom)
					doTrap ();
				else
					doTrap(target);

				isActive = false;
			}
		}
		else
		{
			hazardTimer = 0;
		}
	}

	// Use this for initialization
	public override void doTrap()
	{
		float hazXPos = Random.Range(this.transform.position.x,rangeMax);
		Vector3 hazSpawnLocation = new Vector3(hazXPos, this.transform.position.y, 0);
		Instantiate(hazard, hazSpawnLocation, Quaternion.Euler(Vector3.forward));
	}

	public override void doTrap(GameObject hazTarg)
	{
		if(hazTarg)
		{
			Rigidbody clone;

			Vector3 hazSpawnLocation = hazTarg.transform.position;
			clone = Instantiate(hazard, hazSpawnLocation, Quaternion.Euler(Vector3.forward)) as Rigidbody;
			clone.GetComponent<LazerScript>().getTarget(hazTarg);
		}
	}


}
