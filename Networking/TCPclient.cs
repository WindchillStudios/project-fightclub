using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class TCPclient : MonoBehaviour {

	string input, stringData;
	byte[] writeData;
	TcpClient server;
	public Movement playerMove; //The controller script.
	bool runThread;
	bool thereIsData = false;
	bool dataToWrite = false;
	/*OPEN THE SERVER CONNECTION IN START*/
	void Start(){
		Debug.Log ("Hi");
		server = new TcpClient ("localhost", 8001);
		if (server.Connected) {
			Debug.Log ("Connected");
			runThread = true;
			new Thread (OpenStream).Start();	
		}
	}
	/*CHECKING ON EVERY UPDATE TO SEE IF THERE IS DATA TO BE READ TO OR SENT FROM THE SERVER*/
	void Update(){
		//Checking for data to be used.
		if(thereIsData == true){
			Debug.Log ("thereisdata");
			playerMove.playerInput(stringData); //THIS IS WHERE THE INFORMATION IS PASSED OFF TO THE CONTROLLER SCRIPT.
			thereIsData = false;
		}
		//Checking for data to be sent to the server.
		if(dataToWrite){
			NetworkStream ns2 = server.GetStream();
			Debug.Log ("Data to be wrote");
			if(ns2.CanWrite){
				Debug.Log ("CW");
				ns2.Write(writeData,0,writeData.Length);
				dataToWrite = false;
			}else{
				Debug.Log ("You can not write to the stream.");
			}
		}
	}
	//Prior to sending TO the server, this function encodes a string into bytes.
	public void prepareString(string outputString){
		Debug.Log ("String prep");
		writeData = Encoding.ASCII.GetBytes(outputString);
		dataToWrite = true;
	}
	//A separate thread that is always waiting for information from the server.
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

			stringData = Encoding.ASCII.GetString (data, 0, recv);
			Debug.Log(stringData + " stringdata");
		}
		Debug.Log("Disconnecting from server...");
		ns.Close ();
		server.Close ();
	}

	~TCPclient(){
		Debug.Log ("down");
		runThread = false;
	}
}
