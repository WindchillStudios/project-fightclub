using UnityEngine;
using System.Collections;

public class LazerScript : MonoBehaviour {

	public GameObject explosion;
	public float lazerLife; //How long until the lazer is fired
	float lifeTime; //How long the lazer has been alive
	GameObject target;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (target);
		lifeTime += 1 * Time.deltaTime;

		if(lifeTime < lazerLife)
		{	
			Vector3 direction;

			direction = target.transform.position - this.transform.position;

			Debug.Log(direction);
		
			this.gameObject.transform.Translate(direction/10);
		}

		else if(lifeTime > lazerLife)
		{
			shootLazer();
		}
	}

	void shootLazer(){
		Instantiate(explosion, this.transform.position, this.transform.rotation);
		Destroy(this.gameObject);
	}

	public void getTarget(GameObject targ){
		target = targ;
	}

	void OnCollisionEnter(Collision hit){
		if(hit.gameObject.tag == "Player")
		{
			shootLazer();
		}
	}
}
