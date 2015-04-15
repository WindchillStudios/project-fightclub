using UnityEngine;
using System.Collections;

public class boulderScript : MonoBehaviour {

	float LifeTimer;
	float currentLife;
	public int parentNumber;

	void Start(){
		LifeTimer = 2;
		currentLife = 0;
	}

	void Update(){

		if(currentLife < LifeTimer){
			currentLife += 1*Time.deltaTime;
		}
		else{
			Destroy(this.gameObject);
		}
	}

	void OnCollisionEnter(Collision hit)
	{
		if(hit.gameObject.tag == "Player")
		{
			if(parentNumber != hit.gameObject.GetComponent<Character>().playerNumber){
				hit.gameObject.GetComponent<Character>().takeDamage(7,this.gameObject);
			}
		}
	}
}
