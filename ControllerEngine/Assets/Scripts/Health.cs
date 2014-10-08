using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	Renderer[] skin;

	float health;
	float maxHealth;
	int invTimer;
	int respawnTimer;

	int deaths;
	public bool isDead;
	bool isHurt;

	public GUIText deathNote;
	public GUITexture healthbar;

	// Use this for initialization
	void Start () {
		skin = this.GetComponentsInChildren<Renderer>();
		maxHealth = 100.0f;
		health = maxHealth;
		isDead = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (isDead) {

			respawnTimer ++;
			if(respawnTimer > 20)
			{
				respawnTimer = 0;
				isDead = false;
				respawn();
			}
		}

		healthbar.transform.localScale = new Vector3((health / 500), 0.1f,0);
		healthbar.transform.position = new Vector3 ((health / 1000)+0.1f, 0.9f, 0);

		if(!isDead){

			if(isHurt)
			{
				foreach(Renderer mat in skin)
				{
					mat.material.color = Color.red;
				}
				invTimer ++;
				
				if(invTimer >= 10){
					isHurt = false;
					invTimer = 0;
				}
			}
			else
			{
				foreach(Renderer mat in skin)
				{
					mat.material.color = Color.white;
				}
			}
		}
	}

	public void takeDamage(float damage, /*Vector3 force,*/ GameObject attacker){

		if(!isHurt){
			if(damage > 0)
			{
				health = health - damage;
				isHurt = true;

				if(health < 1)
				{
					if(!isDead)
						takeDeath(attacker);
				}
			}
		}
	}

	void takeDeath(GameObject attacker)
	{
		deathNote.text = this.gameObject.name + " was killed by " + attacker.name;
		isDead = true;
		deaths ++;
	}

	void respawn(){
		GameObject[] spawnLocs = GameObject.FindGameObjectsWithTag("Respawner");
		int pickSpawn = Random.Range (0, spawnLocs.Length-1);

		this.gameObject.transform.position = spawnLocs [pickSpawn].transform.position;
		health = maxHealth;
	}
}