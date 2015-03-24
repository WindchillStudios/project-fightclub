using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MatchControl : MonoBehaviour {

	/* ----	Players ---- */

	public GameObject[] players;
	GameObject[] spawnLocs;
	float countdownTimer;
	bool isCountdown;

	public GameObject Liara;
	public GameObject Durain;
	public GameObject Magna;

	public OSC_Receiver_C heartRate;

	public int[] playerHeartRates;

	/* ----	Heart Rate Traps ---- */

	public float aoeTrapFrequency;
	public GameObject[] aoeTraps;

	public float soloTrapFrequency;
	public GameObject[] soloTraps;

	int avHeartRate;
	float heartTrapDelay;
	bool isHeartTrapActive = false;
	bool canHeartTrap = false;

	int[] playerType;

	public string[] mobileInput;

	/* ---- Level Gui ---- */

	public float[] playerHealths;
	Canvas levelGui;
	public GameObject[] playerBoxes;
	public Image[] healthBars;

	/* ---- Audio ---- */

	public AudioClip[] music;

	/* ---- Game Scoring ---- */


	// Use this for initialization
	void Start () {
		players = new GameObject[4];
		DontDestroyOnLoad (this.gameObject);
		countdownTimer = 4;
	}

	void Awake(){
		if (music.Length > 0) {
			setMusic ();
		}

		if (Application.loadedLevelName != "MainMenu") {
			onLevelLoad ();
		}
	}

	void OnLevelWasLoaded(){
		if (music.Length > 0) {
			setMusic ();
		}

		if (Application.loadedLevelName != "MainMenu") {
			onLevelLoad ();
		}
	}

	void onLevelLoad(){

		if (GameObject.FindObjectOfType<TrapContainer> ())
		{
			aoeTraps = GameObject.FindObjectOfType<TrapContainer> ().allAoeTraps;
			soloTraps = GameObject.FindObjectOfType<TrapContainer> ().allSoloTraps;
		}
		heartRate = GameObject.FindObjectOfType<OSC_Receiver_C> ();
		spawnLocs = GameObject.FindGameObjectsWithTag ("Respawn");

		SpawnPlayers ();
		isCountdown = true;

		levelGui = FindObjectOfType<Canvas> ();

		foreach(GameObject player in players){

			if(player){
			
				player.GetComponent<Character>().setFrozen(true);

				int num = (player.GetComponent<Character>().playerNumber - 1);
				GameObject p = Instantiate(playerBoxes[num]) as GameObject;
				p.transform.SetParent(levelGui.transform, false);
			}
		}

		GameObject[] healthBarObjs = GameObject.FindGameObjectsWithTag ("healthBar");
		healthBars = new Image[healthBarObjs.Length];

		for (int i = 0; i < healthBarObjs.Length; i++){
			healthBars[i] = healthBarObjs[i].GetComponent<Image>();
		}

		playerHealths = new float[healthBars.Length];
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.LoadLevel("MainMenu");
			Destroy(this.gameObject);
		}

		if (Application.loadedLevelName != "MainMenu") {
			ifSingle ();
			getHeartRates();
			updateHud();
			avHeartRate = getAvHeart ();
			heartRateTrapSystem ();
		}

		if(isCountdown){

			if(countdownTimer > 0){
				countdownTimer -= Time.fixedDeltaTime;
				if(countdownTimer > 1){
					levelGui.GetComponentInChildren<Text>().text = Mathf.Floor(countdownTimer).ToString();
				}
				else if(countdownTimer > 0){
					levelGui.GetComponentInChildren<Text>().text = "Fight!";
				}
			}
			else{
				levelGui.GetComponentInChildren<Text>().text = "";

				foreach(GameObject player in players){

					if(player != null){
						player.GetComponent<Character>().setFrozen(false);
					}
				}
				isCountdown = false;
			}
		}
		else{
			countdownTimer = 4;
		}
	}

	/* ----- Get Player Inputs ----- */

	public void getInput(string curInput){

		int controlNum;
		string controlType;
		string controlInput;
		string[] controlData = new string[0];

		controlData = curInput.Split(":"[0]);

		mobileInput = controlData;

		if(controlData.Length >= 3){

			bool result;

			result = int.TryParse(controlData [0], out controlNum);
			if(!result){
				Debug.Log("Unexpected result is " + controlData[0]);
			}

			controlType = controlData [1];
			controlInput = controlData [2];

			sendInput (controlNum, controlType, controlInput);
		}
	}

	void sendInput(int playNum, string type, string contrInput){
		players[playNum].GetComponent<Character>().readInput(type, contrInput);
	}

	void getHeartRates(){
		int curHeart = heartRate.message1;

		if(playerHeartRates.Length > 0)
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
			if(aoeTraps.Length > 0){
				doAoeTrap();
			}
		}
	}

	void doAoeTrap(){
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

	void SpawnPlayers(){

		for(int i = 0; i < players.Length; i++)
		{
			if(players[i] != null){
				spawnLocs = GameObject.FindGameObjectsWithTag ("Respawn");
						
				int pickSpawn = Random.Range (0, spawnLocs.Length);
				spawnLocs[pickSpawn].GetComponentInChildren<ParticleSystem>().Play();
				players[i] = Instantiate(players[i], spawnLocs[pickSpawn].transform.position, spawnLocs[pickSpawn].transform.rotation) as GameObject;
				players[i].GetComponent<Character>().playerNumber = i+1;
				spawnLocs[pickSpawn].tag = "Despawn";
			}
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

	void updateHud(){

		int playCount = 0;

		foreach (GameObject player in players) {

			if(player != null){
				playerHealths[playCount] = player.GetComponent<Character>().getHealth();
				playerHealths[playCount] = playerHealths[playCount] / player.GetComponent<Character>().maxHealth;

				playCount = playCount + 1;
			}
		}

		for(int i = 0; i < playerHealths.Length; i++){
			healthBars[i].transform.localScale = new Vector3(1,playerHealths[i],1);
		}


	}

	void setMusic(){
		
		switch(Application.loadedLevelName){
		case "MainMenu":
			this.GetComponent<AudioSource>().clip = music[0];
			this.GetComponent<AudioSource>().Play();
			break;
		case "AsteroidMine":
			this.GetComponent<AudioSource>().clip = music[2];
			this.GetComponent<AudioSource>().Play();
			break;
		case "TrainingRoom":
			this.GetComponent<AudioSource>().clip = music[1];
			this.GetComponent<AudioSource>().Play();
			break;
		default:
			break;
		}
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
