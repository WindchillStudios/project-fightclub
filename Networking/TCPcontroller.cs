using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class TCPclient : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	roomSelect roomScript;

	public GameObject sendToScript;
	string input, stringData;
	Queue<string> inputs = new Queue<string>();	
	Queue<byte[]> outputs = new Queue<byte[]>();
	bool buffer1full, buffer2full, buffer3full, buffer4full;
	byte[] writeData, writeBuffer1, writeBuffer2, writeBuffer3, writeBuffer4;
	TcpClient server;
	//public Movement playerMove;
	bool runThread;
	bool thereIsData = false;
	bool dataToWrite = false;

	void Start(){
		Debug.Log ("Start");
		server = new TcpClient ("192.168.0.100", 8001);
		if (server.Connected) {
			Debug.Log ("Connected");
			runThread = true;
			new Thread (OpenStream).Start();
			new Thread (DQStream).Start ();
		}
	}

	void Update(){
		if(thereIsData == true){
			//Debug.Log ("thereisdata" + inputs.Dequeue ());
			string inputString = inputs.Dequeue();
			Debug.Log (inputString);
			if(inputString == "Close"){
				Application.LoadLevel ("Title");
			}
			if(inputString == "Win"){
				Application.LoadLevel ("Win");
			}
			if(inputString == "Loss"){
				Application.LoadLevel ("Loss");
			}

			if(inputString == "Progress"){
				sendToScript = GameObject.Find ("Option 1");
				roomScript = sendToScript.GetComponent<roomSelect> ();

				Debug.Log ("Progress");
				roomScript.progressTrue = true;
			}
			if(inputString == "Full"){
				sendToScript = GameObject.Find ("Option 1");
				roomScript = sendToScript.GetComponent<roomSelect> ();

				Debug.Log ("Full");
				roomScript.fullTrue = true;
			}
			if(inputString == "Open"){
				Application.LoadLevel ("Character");
			}
			//playerMove.playerInput(inputs.Dequeue ());
			thereIsData = false;
		}/*
		if(dataToWrite){
				NetworkStream ns2 = server.GetStream();
				Debug.Log ("Data to be wrote");
				if(ns2.CanWrite){
					if(outputs.Count > 0){
					byte[] sentByte = outputs.Dequeue();	
					//Debug.Log ("CW" + sentByte);
					ns2.Write(sentByte, 0, sentByte.Length);
				}else{
					Debug.Log ("No data in outputs");
				}
					if(outputs.Count == 0){
						dataToWrite = false;
					}
				}else{
					Debug.Log ("You can not write to the stream.");
				}
			}*/
	}
	
	public void prepareString(string outputString){
		byte[] encodedString = Encoding.ASCII.GetBytes(outputString);
		Debug.Log ("Encoded " + encodedString.Length);
		outputs.Enqueue(encodedString);
		Debug.Log (outputs.Count);
		dataToWrite = true;
	}
	private void OpenStream(){
		Debug.Log ("STREAM!");
		NetworkStream ns = server.GetStream ();
		byte[] data = new byte[1024];
		int recv;
		while (runThread == true) {
			Debug.Log("Waiting");
			recv = 0;
			try{
				recv = ns.Read (data, 0, data.Length);
				thereIsData = true;
				Debug.Log (recv + " data");
			}
			catch
			{
				Debug.Log("Caught");
				break;
			}
			if(recv == 0){
				Debug.Log ("Breaking down");
				break;
			}
			
			inputs.Enqueue(Encoding.ASCII.GetString (data, 0, recv));
			Debug.Log(stringData + " stringdata");
		}
		Debug.Log("Disconnecting from server...");
		ns.Close ();
		server.Close ();
	}

	private void DQStream(){
		while (runThread == true) {
			if(dataToWrite){
				NetworkStream dq = server.GetStream ();
				Debug.Log ("Data to be wrote");
				if (dq.CanWrite) {
						if (outputs.Count > 0) {
								Debug.Log ("Peek " + outputs.Peek());
								byte[] sentByte = outputs.Dequeue ();	
								dq.Write (sentByte, 0, sentByte.Length);
						} else {
								Debug.Log ("No data in outputs");
						}
						if (outputs.Count == 0) {
								dataToWrite = false;
						}
				} else {
						Debug.Log ("You can not write to the stream.");
			}
			}
		}
	}

	~TCPclient(){
		server.Close ();
	}

	void OnApplicationPause() {
		Debug.Log ("RIPINPEACE");
		writeData = Encoding.ASCII.GetBytes("RIP");
		NetworkStream ns2 = server.GetStream();
		ns2.Write(writeData,0,writeData.Length);
		Application.Quit ();
	}
}
