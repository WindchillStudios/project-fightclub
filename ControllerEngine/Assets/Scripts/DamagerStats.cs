using UnityEngine;
using System.Collections;

public class DamagerStats : MonoBehaviour {

	public float currentDamage;
	public float baseDamage;

	public float maxDamage;
	public float minDamage;

	public bool staticDamage;
	public Health hitHealth;

	float force;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(staticDamage)
		{
			currentDamage = baseDamage;
		}
		else{
			if(this.rigidbody)
			{							
				if(this.rigidbody.mass > 1 && this.rigidbody.velocity.magnitude > 10)
				{
					currentDamage = baseDamage * (this.rigidbody.velocity.magnitude);
				}
				else {
					currentDamage = Random.Range(maxDamage, minDamage);
				}
			}
		}
	}

	void OnCollisionEnter(Collision hit){

		if(hit.collider.tag == "Player")
		{
			if(hit.gameObject.GetComponent<Health>()){
				hit.gameObject.GetComponent<Health> ().takeDamage(currentDamage, this.gameObject);
			}
		}
	}
}
