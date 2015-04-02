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

	public bool isOnline;
	public bool isSet;

	MatchControl gameController;

	string input, stringData;

	Queue<string> inputs = new Queue<string>();	
	Queue<byte[]> outputs = new Queue<byte[]>();
	byte[] writeData, writeData2;

	TcpClient server;

	bool runThread;
	bool thereIsData = false;
	bool dataToWrite = false;

	void Start(){

		if(!isSet){
			DontDestroyOnLoad(this);

			try{
				Debug.Log("Trying to Connect");
				server = new TcpClient ("192.168.0.100", 8001);
				isOnline = true;
				Debug.Log("Found Connection ");
			}
			catch(SocketException e){
				Debug.Log("Can't Connect");
				if (e.Source != null){
					isOnline = false;
				}
			}

			if(isOnline){
				if (server.Connected) {
					runThread = true;
					new Thread (OpenStream).Start();
					prepareString ("Game");
				}
			}
			isSet = true;
			Application.LoadLevel ("MainMenu");
		}
	}

	void FixedUpdate(){
		if(thereIsData == true){
			//Debug.Log ("TCP " + stringData);

			if(inputs.Count != 0){
				Debug.Log("dequeue " + inputs.Peek());

				gameController.getInput(inputs.Dequeue ());
				if(inputs.Count > 50){
					inputs.Clear();
				}
			}

			if(inputs.Count == 0){
				thereIsData = false;
			}
		}
		else{
			if(gameController){
				gameController.getInput("No Mobile Input");
			}
		}

		if(dataToWrite){
			if(server != null){
				NetworkStream ns2 = server.GetStream();
				//Debug.Log ("Data to be wrote");
				if(ns2.CanWrite){
					//Debug.Log ("CW");
					byte[] outputByte = outputs.Dequeue();
					ns2.Write(outputByte,0,outputByte.Length);
					if(outputs.Count == 0){
						dataToWrite = false;
					}
				
				}else{
					//Debug.Log ("You can not write to the stream.");
				}
			}
		}
	}

	void OnLevelWasLoaded(){
		if (!gameController) {
				gameController = GameObject.FindObjectOfType<MatchControl> ();
		}
	}

	public void prepareString(string outputString){
		byte[] encodedString = Encoding.ASCII.GetBytes (outputString);
		outputs.Enqueue(encodedString);
		dataToWrite = true;
	}

	private void OpenStream(){
		//Debug.Log ("STREAM!");
		NetworkStream ns = server.GetStream ();
		byte[] data = new byte[1024];
		int recv;
		while (runThread == true) {
			//Debug.Log("Waiting");
			recv = 0;
			try{
				recv = ns.Read (data, 0, data.Length);
				thereIsData = true;
				//Debug.Log (recv + " data");
			}
			catch
			{
				//Debug.Log("Caught");
				break;
			}
			if(recv == 0){
				//Debug.Log ("Breaking down");
				break;
			}

			inputs.Enqueue(Encoding.ASCII.GetString (data, 0, recv));
			//Debug.Log(stringData + " stringdata");
		}
		//Debug.Log("Disconnecting from server...");
		ns.Close ();
		server.Close ();
	}

	~TCPclient(){
		runThread = false;
		server.Close ();
	}

	void OnApplicationQuit(){

		runThread = false;
		writeData = Encoding.ASCII.GetBytes("Close Game");
		
		if (server != null) {
			NetworkStream ns2 = server.GetStream ();
			ns2.Write(writeData,0,writeData.Length);

			server.Close ();
		}

		Application.Quit ();
	}
}
