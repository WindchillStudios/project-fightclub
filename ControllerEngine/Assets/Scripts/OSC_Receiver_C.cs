using UnityEngine;
using System.Collections;

public class OSC_Receiver_C : MonoBehaviour {

	public string RemoteIP = "127.0.0.1";
	public int SendToPort = 57131;
	public int ListenerPort = 57130;

	public Osc handler;
	public GUIText textOutput;
	public Transform spawnObj;

	public int message1;
	public int message2;

	// Use this for initialization
	void Start () {
		// Set up OSC connection
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init(RemoteIP, SendToPort, ListenerPort);
		handler = GetComponent<Osc> ();
		handler.init(udp);
		
		// Listen to the channels set in the Processing sketch
		handler.SetAddressHandler("/pulseVals", ListenEvent);
	}
	
	// Update is called once per frame
	void Update () {

//		Debug.Log ("BPM1: " + message1 + "BPM2 " + message2);

		if(textOutput){
			textOutput.text = "BPM: " + message1 + "BPM2 " + message2;
		}
	}

	public void ListenEvent(OscMessage oscMessage)
	{	
		// Make the data available 
		message1 = (int)oscMessage.Values[0];
		message2 = (int)oscMessage.Values [2];
	}
}