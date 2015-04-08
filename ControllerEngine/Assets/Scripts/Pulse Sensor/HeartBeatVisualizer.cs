using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Attach this and the BeatScript to the object you want to beat.
/// </summary>

public delegate void HBEvent();

public class HeartBeatVisualizer : MonoBehaviour {

	public GameObject Receiver;

	public int tempBPM; //temporary placeholder value for BPM
	private List<int> listBPM = new List<int>(); //list of BPM values to be averaged
	private int avgBPM; //averaged BPM value (over a time period and a series of values)
	public int BPM; //final BPM value that will be used to find an interval

	private int listIncrement; //int used to track increments of the HBManager coroutine
	private float interval; //value for the time between beats
	private float nextTime; //next time to trigger a beat, incremented by interval

	public event HBEvent OnBeat; //event to trigger an on beat
	public event HBEvent OffBeat; //event to trigger off the beat

	void Start () {
		Receiver = GameObject.Find("OSC Receiver"); //change this based on what is holding the OSC_Receiver script

		tempBPM = 60; //temp value to prevent divide by zero errors
		BPM = 60; //get BPM from the reciever, grab the message based on what player this is
		avgBPM = 0; //set stuff to defaults
		interval = 1;
		listIncrement = 0;
		if (Receiver) {
			StartHBManager ();//start the coroutines
		}
		StartHBVis (); 
	}

	public void StartHBManager(){ //coroutine for interpreting and setting up the interval value
		StopCoroutine ("HBManager"); //makes sure it isnt already running
		StartCoroutine ("HBManager");
	}

	public void StartHBVis(){ //coroutine for creating beats based on the interval
		StopCoroutine ("HBeat");
		nextTime = Time.time;
		StartCoroutine ("HBeat");
	}

	IEnumerator HBManager(){
		for (; ;) {
			switch (this.gameObject.name){
			
			case "p1Heart":
				tempBPM = Receiver.GetComponent<OSC_Receiver_C>().player1BPM; //get the BPM from the OSC reciever
				//Debug.Log (this.gameObject.name + " : " + tempBPM);
				break;

			case "p2Heart":
				tempBPM = Receiver.GetComponent<OSC_Receiver_C>().player2BPM;
				//Debug.Log (this.gameObject.name + " : " + tempBPM);
				break;

			case "p3Heart":
				tempBPM = Receiver.GetComponent<OSC_Receiver_C>().player3BPM;
				//Debug.Log (this.gameObject.name + " : " + tempBPM);
				break;

			case "p4Heart":
				tempBPM = Receiver.GetComponent<OSC_Receiver_C>().player4BPM;
				//Debug.Log (this.gameObject.name + " : " + tempBPM);
				break;

			}

			listBPM.Add(tempBPM); //add that OSC value to the list of BPM values

			avgBPM += listBPM[listIncrement]; //add values to the average from an incremented postition in the list

			if (listIncrement == 4){ //stop the increments at 5 increments (seconds, can be changed depending on how often you want a new average
				avgBPM = avgBPM/5; //divide the accumlulated value to find the real average
				BPM = avgBPM; //set a new value of BPM
				avgBPM = 0; //reset the average holder
				listBPM = new List<int>(); //reset the list
			}

			yield return new WaitForSeconds(1); //wait for one second
			//stop and reset increment accordingly (in order to get a new average every 5s, or whatever interval you choose)
			if (listIncrement < 4){ //be sure to change this to match the previous if statement (number of seconds, etc)
				listIncrement++;
			}
			else{
				listIncrement = 0;
			}
		}
	}
	
	IEnumerator HBeat(){

		for (; ;) { //set loops like this to have a conditional end point per player (eg. death or on scene end maybe)
			
			if (OnBeat != null){
				OnBeat(); //runs function in Beatscript
			}
			interval = 60 / (float)BPM; //get the interval from the BPM and base seconds
			nextTime += interval; //increment the next time value
			float waitTime = (nextTime - Time.time) / 2; //determine half the time we will wait between beats
			yield return new WaitForSeconds(waitTime); //wait between beats

			//waiting for half the time so that we can have on and off beat calls, simply calls one half, waits, calls second half, waits, repeat
			if (OffBeat != null){
				OffBeat(); //runs function in Beatscript
			}
			
			yield return new WaitForSeconds(waitTime); //wait the second half of the delay
		}
	}
}
