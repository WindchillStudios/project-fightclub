using UnityEngine;
using System.Collections;

public class enemyBehaviour : MonoBehaviour {

	//Objects in game
	public Transform gravitySource;
	public Transform playerLoc;
	public Transform patrolPoint;
	public GUIText gameText;
	public GUIText healthText;
	//public Rigidbody enemyBullet;
	private Vector3 playerLocVal;
	private Vector3 homeLocVal;



	//Movement variables
	//private float rotateSpeed;
	private float orbitDegrees;
	//private float direction;

	private float chaseSpeed;
	private float returnSpeed;

	//Distance between enemy and objects
	private float distanceToShip;
	private float distanceToHome;

	//Enemy statistics
	private float enemyHealth;
	private bool shipHit;
	private bool isTiming = false;
	private float counter = 0;
	
	// Use this for initialization
	void Start () {
		//rotateSpeed = 10;
		orbitDegrees = 1;
		//direction = 1;

		chaseSpeed = 6;
		returnSpeed = 7;

		enemyHealth = 60;

		shipHit = false;

		gameText.text = "Find and attempt to land on the green dot";

	}
	
	// Update is called once per frame
	void Update () {
		//transform.Rotate(new Vector3(0, direction, 0), rotateSpeed * Time.deltaTime);

		distanceToShip = Vector3.Distance(transform.position,playerLoc.transform.position);
		distanceToHome = Vector3.Distance(transform.position,gravitySource.transform.position);

		//Debug.Log("Distance to Ship:" + distanceToShip);
		//Debug.Log ("Distance to Home" + distanceToHome);

		hitTimer ();

		healthText.text = "Enemy Health: " + enemyHealth;

		if(enemyHealth <= 20){
			returnHome();
		}
			//ORBITING PLANET
			if(distanceToShip >= 15){
				//STATE 1 - RETURN HOME IF PLAYER GETS AWAY
				if(distanceToHome >= 5){
					returnHome();
				}
				//STATE 2 - ORBIT HOME PLANET
				else{
					orbit();
					//Debug.Log("Is Orbiting");
				}
			}

			//ATTACKING PLAYER
			else if(distanceToShip < 20){




				//STATE 4 - CHASE PLAYER IF THEY GET CLOSER || STOP IF TOO CLOSE
				if(distanceToShip < 15 /*&& distanceToShip > 2*/){
					if(shipHit == false){
						chasePlayer();
					}
				}
				//Debug.Log("Is Chasing");

				//STATE 5 - SHOOT AT PLAYER IF THEY GET EVEN CLOSER
	//			if(distanceToShip <= 10){
	//				InvokeRepeating("shootMissile", 0, 1000);
	//			}

			}


	
	}

	void orbit(){
		transform.RotateAround(gravitySource.position, new Vector3(0, 1, 0), (orbitDegrees)*-1);
		//transform.Rotate (0, 90, 0);
		//restore health if not full
		if (enemyHealth < 100) {
			enemyHealth += 10;
		}
	}

	void returnHome(){

		homeLocVal = gravitySource.position;
		homeLocVal.y = 0;
		transform.LookAt (homeLocVal);

		//CONVERT TO FORCES MAYBE?
		transform.position = Vector3.MoveTowards (new Vector3(transform.position.x, 0, transform.position.z), gravitySource.transform.position, returnSpeed * Time.deltaTime);
	}

	void chasePlayer(){

		//STATE 3 - LOOK AT PLAYER IF THEY APPROACH
		playerLocVal = playerLoc.position;
		playerLocVal.y = 0;
		transform.LookAt (playerLocVal);

		//Move towards player
		transform.position = Vector3.MoveTowards (new Vector3(transform.position.x, 0, transform.position.z), playerLoc.transform.position, chaseSpeed * Time.deltaTime);


	}

	void OnCollisionEnter (Collision collision){
		if (collision.collider.tag == "Player") {
			shipHit = true;
			enemyHealth -= 10;
			hitTimerStart();
			Debug.Log ("Hit Player: " + enemyHealth);
		}
	}

	void hitTimerStart(){
		counter = 0;
		isTiming = true;
	}

	void hitTimer(){
		if(isTiming){
			counter += Time.deltaTime;
			//Debug.Log ("Counter: " + counter);
		}
		if (counter >= 2) {
			shipHit = false;
			hitTimerEnd();
		}
	}
	
	void hitTimerEnd(){
		isTiming = false;
	}

//	void shootMissile(){
//		for (int i = 0; i <= 1; i++){
//			Rigidbody clone;
//			clone = Instantiate(enemyBullet, transform.position + new Vector3(2,0,0), transform.rotation) as Rigidbody;
//			clone.velocity = transform.TransformDirection(Vector3.forward * 10);
//			CancelInvoke("shootMissile");
//		}
//	}
}
