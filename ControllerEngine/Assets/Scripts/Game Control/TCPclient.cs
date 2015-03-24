using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class TCPclient : MonoBehaviour {

	public bool isOnline;

	MatchControl gameController;

	string input, stringData;
	byte[] writeData;

	TcpClient server;
	bool runThread;
	bool thereIsData = false;
	bool dataToWrite = false;

	void Start(){
		gameController = this.GetComponent<MatchControl> ();

		if(isOnline){
			Debug.Log ("Online");

			server = new TcpClient ("192.168.0.100", 8001);

			if (server.Connected) {
				//Debug.Log ("Connected");
				runThread = true;
				new Thread (OpenStream).Start();
				prepareString ("Game");
			}
		}
	}

	void Update(){
		if(thereIsData == true){
			//Debug.Log ("TCP " + stringData);

			gameController.getInput(stringData);
			thereIsData = false;
		}
		else
			gameController.getInput("No Mobile Input");

		if(dataToWrite){
			NetworkStream ns2 = server.GetStream();
			//Debug.Log ("Data to be wrote");
			if(ns2.CanWrite){
				//Debug.Log ("CW");
				ns2.Write(writeData,0,writeData.Length);
				dataToWrite = false;
			}else{
				//Debug.Log ("You can not write to the stream.");
			}
		}
	}

	public void prepareString(string outputString){
		//Debug.Log ("String prep");
		writeData = Encoding.ASCII.GetBytes(outputString);
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

			stringData = Encoding.ASCII.GetString (data, 0, recv);
			//Debug.Log(stringData + " stringdata");
		}
		//Debug.Log("Disconnecting from server...");
		ns.Close ();
		server.Close ();
	}

	~TCPclient(){
		//Debug.Log ("down");
		runThread = false;
	}
}
