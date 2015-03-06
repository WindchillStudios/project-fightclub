using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
	
	float lifeTime;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		lifeTime += 1 * Time.deltaTime;
		
		if (lifeTime > 3)
			Destroy (this.gameObject);	
	}
	
	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.tag == "Player")
		{
			Vector3 direction = hit.ClosestPointOnBounds(this.transform.position);
			hit.gameObject.GetComponent<Character>().knockBack(direction);
			hit.gameObject.GetComponent<Character>().takeDamage(10,this.gameObject);
		}
	}
}
