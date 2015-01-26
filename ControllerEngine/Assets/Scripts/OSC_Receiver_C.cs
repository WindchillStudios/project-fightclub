using UnityEngine;
using System.Collections;

public class OSC_Receiver_C : MonoBehaviour {

	public string RemoteIP = "127.0.0.1";
	public int SendToPort = 57131;
	public int ListenerPort = 57130;

	public Osc handler;
	public GUIText textOutput;
	public Transform spawnObj;

	public int message;

	// Use this for initialization
	void Start () {
		// Set up OSC connection
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init(RemoteIP, SendToPort, ListenerPort);
		handler = GetComponent<Osc> ();
		handler.init(udp);
		
		// Listen to the channels set in the Processing sketch
		handler.SetAddressHandler("/bpmNum", ListenEvent);
	}
	
	// Update is called once per frame
	void Update () {
		textOutput.text = "BPM: " + message;
	}

	public void ListenEvent(OscMessage oscMessage)
	{	
		// Make the data available 
		message = (int)oscMessage.Values[0];
		
	} 
}