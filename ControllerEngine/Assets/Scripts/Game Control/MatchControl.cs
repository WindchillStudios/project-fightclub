using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MatchControl : MonoBehaviour {

	/* ----	Players ---- */

	public GameObject[] players;
	GameObject[] spawnLocs;
	float countdownTimer;
	bool isCountdown;

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
	TrapScript trapController; 

	int[] playerType;

	public string[] mobileInput;

	/* ---- Level Gui ---- */

	public float[] playerHealths;
	Canvas levelGui;
	public GameObject[] playerBoxes;
	public Image[] healthBars;
	public Sprite[] portraits;

	/* ---- Audio ---- */

	public AudioClip[] music;
	public AudioClip[] announcer;

	/* ---- Game Scoring ---- */

	int selGameType = 2;
	int[] kills;
	float matchTimer = 20;
	GameObject timerDisplay;
	bool timerSet;
	Font OutageFont;
	float endTimer = 2;
	bool isEnding;
	int[] results;
	bool fightPlayed = false;

	// Use this for initialization
	void Start () {
		OutageFont = Resources.Load("Outage") as Font;
		players = new GameObject[4];
		results = new int[4];
		DontDestroyOnLoad (this.gameObject);
		countdownTimer = 4;
	}

	void Awake(){
		if (music.Length > 0) {
			setMusic ();
		}

		if (Application.loadedLevelName != "MainMenu" && Application.loadedLevelName != "Results") {
			onLevelLoad ();
		}
	}

	void OnLevelWasLoaded(){
		if (music.Length > 0) {
			setMusic ();
		}

		if (Application.loadedLevelName != "MainMenu" && Application.loadedLevelName != "Results") {
			onLevelLoad ();
		}
		else if(Application.loadedLevelName == "Results"){
			createResults();
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

				Transform portrait = p.transform.FindChild("PlayerPortrait");

				if(player.GetComponent<Bandit>()){
					portrait.GetComponent<Image>().sprite = portraits[0];
				}
				else if(player.GetComponent<Paladin>()){
					portrait.GetComponent<Image>().sprite = portraits[1];
				}
				else if(player.GetComponent<Golem>()){
					portrait.GetComponent<Image>().sprite = portraits[2];
				}
				else if(player.GetComponent<Dragon>()){
					portrait.GetComponent<Image>().sprite = portraits[3];
				}
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
			foreach(GameObject player in players){
				Destroy(player);
			}

			if(GameObject.FindObjectOfType<TCPclient>() && GameObject.FindObjectOfType<TCPclient>().isOnline){
				GameObject.FindObjectOfType<TCPclient>().prepareString ("Close Menu");
			}
			Application.LoadLevel("MainMenu");
			Destroy(this.gameObject);
		}

		if (Application.loadedLevelName != "MainMenu" && Application.loadedLevelName != "Results") {

			if(heartRate){
				getHeartRates();
			}
			updateHud();

			if(!isCountdown){
				updateWinConditions ();
			}

			avHeartRate = getAvHeart ();
			heartRateTrapSystem ();

			if(isCountdown){

				if(countdownTimer > 0){
					countdownTimer -= Time.fixedDeltaTime;
					if(countdownTimer > 1){

						if(countdownTimer < 2){
							if(!fightPlayed){
								this.GetComponent<AudioSource>().PlayOneShot(announcer[0],1.0f);
								fightPlayed = true;
							}
						}
						if(levelGui){
							levelGui.GetComponentInChildren<Text>().text = Mathf.Floor(countdownTimer).ToString();
						}
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

		if (isEnding) {

			endTimer -= 1 * Time.fixedDeltaTime;

			if(endTimer <= 0){
				if(FindObjectOfType<TCPclient>()){
					GameObject.FindObjectOfType<TCPclient>().prepareString ("Close Over " + (results[0]-1));
				}
				Application.LoadLevel ("Results");
				isEnding = false;
			}
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
		if(players.Length > 0){
			players[playNum].GetComponent<Character>().readInput(type, contrInput);
		}
	}

	void getHeartRates(){
		int curHeart = heartRate.message1;

		if(playerHeartRates.Length > 0)
			playerHeartRates[0] = curHeart;
	}

	void heartRateTrapSystem()
	{
		if (isHeartTrapActive) {
			Transform warningScreen = levelGui.transform.FindChild ("WarningScreen");
			warningScreen.gameObject.SetActive (true);
		}
		else{
			Transform warningScreen = levelGui.transform.FindChild ("WarningScreen");
			warningScreen.gameObject.SetActive (false);
		}

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
		
		int randTrap = Random.Range (0, (aoeTraps.Length));

		if (!isHeartTrapActive) {
			trapController = aoeTraps [randTrap].GetComponent<TrapScript> ();
		}

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
				players[i].GetComponent<Character>().setGameType(selGameType);
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

	void updateWinConditions(){
		switch(selGameType){
		
		case 0: // SURVIVAL

			int playersAlive = 0;
		
			foreach(GameObject player in players){
				if(player != null && player.activeSelf == true){
					playersAlive += 1;
				}
			}

			if(playersAlive < 2){

				foreach(GameObject player in players){
					if(player){
						if(player.GetComponent<Character>()){
							if(player.GetComponent<Character>().readLives() > 0){
								results[0] = player.GetComponent<Character>().playerNumber;
							}
							/*else{
								if(results[1] == null){
									results[1] = player.GetComponent<Character>().playerNumber;
								}else{
									if(results[2] == null){
										results[2] = player.GetComponent<Character>().playerNumber;
									}
									else if(results[3] == null){
										results[3] = player.GetComponent<Character>().playerNumber;
									}
								}
							}*/
						}
					}
				}

				getResults();
			}
		
			break;

		case 1: // TIMED DM
		
			getLeader();

			if(!timerSet){
			
				timerDisplay = new GameObject();
				Text timerText = timerDisplay.AddComponent<Text>();
				timerDisplay.AddComponent<Outline>();

				timerDisplay.transform.SetParent(levelGui.transform);

				timerText.rectTransform.localPosition = new Vector3(0,-180,0);
				timerText.rectTransform.localScale = new Vector3(1,1,1);
				timerText.alignment = TextAnchor.MiddleCenter;
				timerText.fontSize = 40;
				timerText.font = OutageFont;
				timerText.color = Color.white;

				timerSet = true;
			}

			string seconds;

			if(Mathf.Floor(matchTimer%60) < 10){
				seconds = "0" + Mathf.Floor(matchTimer%60).ToString();
			}
			else{
				seconds = Mathf.Floor(matchTimer%60).ToString();
			}

			timerDisplay.GetComponent<Text>().text = Mathf.Floor(matchTimer/60).ToString() + ":" + seconds;

			if(matchTimer > 1){
				matchTimer -= (1 * Time.fixedDeltaTime);
			}
			else if (matchTimer < 1){
				getResults();
			}
			break;

		case 2: // SCORED DM

			getLeader();

			foreach(GameObject player in players){
				if(player != null){
					if(player.GetComponent<Character>().readKills() > 9){
						getResults();
					}
				}
			}
			break;
		}
	}

	public void setThisGameType(int setType){
		selGameType = setType;
	}

	void getLeader(){

		int maxKills = 0;
		//int secondKills = 0;
		//int thirdKills = 0;

		foreach(GameObject player in players){
			if(player){
				if(player.GetComponent<Character>().readKills() > maxKills){
					results[0] = player.GetComponent<Character>().playerNumber;
					maxKills = player.GetComponent<Character>().readKills();
				}
				/*else{
					if(player.GetComponent<Character>().readKills() > secondKills){
						results[1] = player.GetComponent<Character>().playerNumber;
						secondKills = player.GetComponent<Character>().readKills();
					}
					else{
						if(player.GetComponent<Character>().readKills() > thirdKills){
							results[2] = player.GetComponent<Character>().playerNumber;
							thirdKills = player.GetComponent<Character>().readKills();
						}
						else{
							results[3] = player.GetComponent<Character>().playerNumber;
						}
					}
				}*/
			}
		}
	}

	void getResults(){

		levelGui.GetComponentInChildren<Text> ().fontSize = 80;
		levelGui.GetComponentInChildren<Text>().text = "COMPLETE!";

		foreach(GameObject player in players){
			if(player){
				player.GetComponent<Character>().setFrozen(true);

				DontDestroyOnLoad(player);
			}
		}

		isEnding = true;
	}

	void createResults(){

		foreach (GameObject player in players) {
			if(player){

				if(player.activeInHierarchy == false){
					player.GetComponent<Character>().setLives(1);
					player.SetActive(true);
				}

				if(player.GetComponent<Character>().playerNumber == results[0]){

					int randmTaunt = Random.Range(0,player.GetComponent<Character>().taunts.Length);
					player.GetComponent<AudioSource> ().PlayOneShot (player.GetComponent<Character>().taunts[randmTaunt], 1.0f);

					player.transform.position = new Vector3(-15,-5,0);
					player.transform.rotation = new Quaternion(0,180,0,0);
					player.transform.localScale = new Vector3(2.5f,2.5f,2.5f);
				}
				else{
					if(player.GetComponent<Character>()){
						player.transform.position = new Vector3((player.GetComponent<Character>().playerNumber*10)-15,-5,0);
						player.transform.rotation = new Quaternion(0,180,0,0);
						player.transform.localScale = new Vector3(2,2,2);
					}
				}
			}
		}
	}

	void setMusic(){
		
		switch(Application.loadedLevelName){
		case "MainMenu":
			this.GetComponent<AudioSource>().clip = music[0];
			this.GetComponent<AudioSource>().Play();
			break;
		case "AsteroidMine":
			this.GetComponent<AudioSource>().clip = music[1];
			this.GetComponent<AudioSource>().Play();
			break;
		case "TrainingRoom":
			this.GetComponent<AudioSource>().clip = music[2];
			this.GetComponent<AudioSource>().Play();
			break;
		case "CityStreet":
			this.GetComponent<AudioSource>().clip = music[3];
			this.GetComponent<AudioSource>().Play();
			break;
		case "Results":
			this.GetComponent<AudioSource>().clip = music[4];
			this.GetComponent<AudioSource>().Play();
			break;
		default:
			break;
		}
	}
}
