using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {

	//Attach this to your OSC handler object or something similar
	//Here get a series of bools from individual scripts that turn true when AVG BPM is higher than a threshold, then use them to trigger events

	//Uncomment p3 and p4 things when you need it.

	public GameObject player1; //Getting the game objects for each player (the same object the visualizer and beat script are attached to)
	public GameObject player2;
	public GameObject player3;
	public GameObject player4;

	private List<bool> playerTriggers = new List<bool>(); //Bool to set based on received data from the Heat rate manager

	private int triggerCounter;

	public bool eventReady;

	void Start () {
		player1 = GameObject.Find ("p1Heart"); //find each player object in the scene
		player2 = GameObject.Find ("p2Heart");
		player3 = GameObject.Find ("p3Heart");
		player4 = GameObject.Find ("p4Heart");

		//Set defaults
		for(int i = 0; i <= 3; i++){
			playerTriggers.Insert(i, false);
		}

		eventReady = true;

		triggerCounter = 0;
	}

	void Update () {

		//Get and set the triggered condition from the Heart Rate Manager and determine which are in the scene
		if (player1){
			playerTriggers[0] = player1.GetComponent<HeartRateManager>().underThreshold;
		}
		if (player2){
			playerTriggers[1] = player2.GetComponent<HeartRateManager>().underThreshold;
		}
		if (player3){
			playerTriggers[2] = player3.GetComponent<HeartRateManager>().underThreshold;
		}
		if (player4){
			playerTriggers[3] = player4.GetComponent<HeartRateManager>().underThreshold;
		}

		if (eventReady == true) { //no recent events, etc

			if (playerTriggers[0] == true || playerTriggers[1] == true || playerTriggers[2] == true || playerTriggers[3] == true) {

				for(int i = 0; i <= 3; i++){
					if(playerTriggers[i] == true){
						triggerCounter++;
					}
				}

				Debug.Log (triggerCounter + "Players triggered");

				if (triggerCounter > 1) {
					//trigger map-wide event
					Debug.Log ("MapWide Event");
					eventReady = false; //Reset because event just happened, this needs to be set back to true somehow
				}

				else {
					//trigger player specific event, not all this stuff is optimised to use the array/list efficiently
					if(playerTriggers[0] == true){
						Debug.Log ("P1 Target Event");
					}
					if(playerTriggers[1] == true){
						Debug.Log ("P2 Target Event");
					}
					if(playerTriggers[2] == true){
						Debug.Log ("P3 Target Event");
					}
					if(playerTriggers[3] == true){
						Debug.Log ("P4 Target Event");
					}

					eventReady = false; //Reset because event just happened, this needs to be set back to true somewhere when the game is ready again
				}
				
			}
		}
	}
}
