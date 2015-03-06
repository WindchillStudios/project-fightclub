using UnityEngine;
using System.Collections;

public class MatchControl : MonoBehaviour {

	public GameObject[] players;
	public GameObject[] characters;
	GameObject[] spawnLocs;

	public GameObject Liara;
	public GameObject Durain;
	public GameObject Magna;

	public OSC_Receiver_C heartRate;

	public int[] playerHeartRates;

	//Heart Rate Traps //
	public float aoeTrapFrequency;
	public GameObject[] aoeTraps;

	public float soloTrapFrequency;
	public GameObject[] soloTraps;

	int avHeartRate;
	float heartTrapDelay;
	bool isHeartTrapActive = false;
	bool canHeartTrap = false;

	int[] playerType;

	// Use this for initialization
	void Start () {
		spawnLocs = GameObject.FindGameObjectsWithTag ("Respawn");
		SpawnPlayers ();
	}
	
	// Update is called once per frame
	void Update () {

		getHeartRates();
		avHeartRate = getAvHeart ();
		heartRateTrapSystem ();

		ifSingle ();
	}

	void getHeartRates(){
		int curHeart = heartRate.message;
		playerHeartRates[0] = curHeart;
	}

	void heartRateTrapSystem()
	{
		//Default State//
		if(heartTrapDelay > aoeTrapFrequency)
		{
			canHeartTrap = true;
		}

		//Trap Delay State//
		if(!isHeartTrapActive && !canHeartTrap)
		{
			heartTrapDelay += 1 * Time.deltaTime;
		}

		if(canHeartTrap)
		{
			heartTrapDelay = 0;
		}

		//Trap Running State//
		if(avHeartRate < 80)
		{
			TrapScript trapController; 

			int randTrap = Random.Range (0, (aoeTraps.Length-1));
			trapController = aoeTraps[randTrap].GetComponent<TrapScript>();

			if(canHeartTrap && !isHeartTrapActive)
			{
				avPulseTrap(trapController);
			}

			if(isHeartTrapActive && !trapController.GetComponent<TrapScript>().isActive)
			{
				isHeartTrapActive = false;
			}
		}
	}

	void SpawnPlayers(){

		for(int i = 0; i < players.Length; i++)
		{
			spawnLocs = GameObject.FindGameObjectsWithTag ("Respawn");
					
			int pickSpawn = Random.Range (0, spawnLocs.Length);
			spawnLocs[pickSpawn].GetComponentInChildren<ParticleSystem>().Play();
			players[i] = Instantiate(players[i], spawnLocs[pickSpawn].transform.position, spawnLocs[pickSpawn].transform.rotation) as GameObject;
			players[i].GetComponent<Character>().playerNumber = i+1;
			spawnLocs[pickSpawn].tag = "Despawn";
		}

		spawnLocs = GameObject.FindGameObjectsWithTag ("Despawn");

		foreach(GameObject spawn in spawnLocs)
		{
			spawn.tag = "Respawn";
		}
	}

	int getAvHeart(){

		int average = 0;

		foreach(int heart in playerHeartRates)
		{
			average = average + heart;
		}

		if(playerHeartRates.Length != 0)
			average = average / playerHeartRates.Length;

		return average;
	}

	void avPulseTrap(TrapScript trap)
	{
		canHeartTrap = false;
		isHeartTrapActive = true;

		trap.activate();
	}

	void ifSingle(){
		if(players.Length < 2)
		{
			Vector3 lastPos = players[0].transform.position;
			Quaternion lastRot = players[0].transform.rotation;
			
			if(Input.GetKey(KeyCode.Z))
			{
				Destroy(players[0]);
				players[0] = Instantiate(Liara, lastPos, lastRot)  as GameObject;
				players[0].GetComponent<Character>().playerNumber = 1;
			}
			if(Input.GetKey(KeyCode.X))
			{
				Destroy(players[0]);
				players[0] = Instantiate(Durain, lastPos, lastRot)  as GameObject;
				players[0].GetComponent<Character>().playerNumber = 1;			
			}
			if(Input.GetKey(KeyCode.C))
			{
				Destroy(players[0]);
				players[0] = Instantiate(Magna, lastPos, lastRot)  as GameObject;
				players[0].GetComponent<Character>().playerNumber = 1;
			}
		}
	}
}
