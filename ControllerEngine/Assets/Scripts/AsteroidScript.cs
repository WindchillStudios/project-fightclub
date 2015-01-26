using UnityEngine;
using System.Collections;

public class AsteroidScript : MonoBehaviour {
	
	public GameObject explosion;

	float lifeTime;
	
	// Use this for initialization
	void Start () {
		this.rigidbody.AddForce (Vector3.down*50, ForceMode.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
		lifeTime += 1 * Time.deltaTime;
		
		if (lifeTime > 6)
			Destroy (this.gameObject);
	}
	
	void OnCollisionEnter(Collision hit)
	{
		if(hit.gameObject.tag != "Hazard")
		{
			Instantiate(explosion, this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
		}
	}
}
