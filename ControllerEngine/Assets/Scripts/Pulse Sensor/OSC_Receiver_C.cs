using UnityEngine;
using System.Collections;

public class OSC_Receiver_C : MonoBehaviour {

	public string RemoteIP = "127.0.0.1";
	public int SendToPort = 57131;
	public int ListenerPort = 57130;

	public Osc handler;
	public GUIText textOutput;
	public Transform heartObj;

	public int message1;
	public int message2;
	public int message3;
	public int message4;

	public int player1BPM;
	public int player2BPM;
	public int player3BPM;
	public int player4BPM;

	void Start () {

		DontDestroyOnLoad (this.gameObject);

		Debug.Log ("Starting reciever");

		// Set up OSC connection
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init(RemoteIP, SendToPort, ListenerPort);
		handler = GetComponent<Osc> ();
		handler.init(udp);
		
		// Listen to the channels set in the Processing sketch
		handler.SetAddressHandler("/pulseVals", ListenEvent);

		player1BPM = 70; //You can put temp values in here for testing! Or get fancy and put a random value or something
		player2BPM = 40;
		player3BPM = 55;
		player4BPM = 75;
	}

	void Update () {
		// Output to text values (temporary)
		if (textOutput) {
			textOutput.text = "BPM1: " + message1 + " BPM2: " + message2 + " BPM3: " + message3 + " BPM4: " + message4;
		}
			//Debug.Log (player1BPM);
	}

	public void ListenEvent(OscMessage oscMessage)
	{	
		//This might need something to check to see if it's getting values or not (ie if a player is connected) so that it doesnt always transmit?
		//Might need to get changed in the Visualizer script instead, but it might not break and you could just tell it to ignore non-connected player values.

		message1 = (int)oscMessage.Values[0]; //Player 1 BPM
		player1BPM = message1;
		message2 = (int)oscMessage.Values[1]; //Player 2 BPM
		player2BPM = message2;
		message3 = (int)oscMessage.Values[2]; //Player 3 BPM
		player3BPM = message3;
		message4 = (int)oscMessage.Values[3]; //Player 4 BPM
		player4BPM = message4;
		
	} 
}