using UnityEngine;
using System.Collections;

public class HeartRateManager : MonoBehaviour {
	
	public int playerAvgBPM;
	public bool underThreshold; // Can potentially make one for 'over' as well
	private int threshold;
	//threshold will need to be determined by a % or static interval above their initial rate read over the first X seconds
	//this will probably only work if we have a recording period before each match, otherwise just use a static value
	//alternatively we could dynamically change the treshold over time?


	void Start () {
		playerAvgBPM = 0;
		underThreshold = false;
		threshold = 80; //temp value
	}

	void Update () {
		playerAvgBPM = this.GetComponent<HeartBeatVisualizer>().BPM;

		//add increment/counter for interval? eg. if player avg rate remains under for x seconds or cycles
		if (playerAvgBPM <= threshold) {
			if (Time.timeSinceLevelLoad >= 15){ //game time delay
					underThreshold = true; //Can now trigger events
			}
		}
		else{
			underThreshold = false;
		}
	}
}
