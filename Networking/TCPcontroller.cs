using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class TCPclient : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	string input, stringData;
	TcpClient server;
	//public Movement playerMove;
	bool runThread;
	//bool thereIsData = false;
	bool dataToWrite = false;

	bool buffer1full, buffer2full, buffer3full, buffer4full;
	byte[] writeData, writeBuffer1, writeBuffer2, writeBuffer3, writeBuffer4;

	void Start(){
		Debug.Log ("Start");
		server = new TcpClient ("192.168.0.100", 8001);
		if (server.Connected) {
			Debug.Log ("Connected");
			runThread = true;
			//new Thread (OpenStream).Start();	
		}
	}

	void Update(){
		/*if(thereIsData == true){
			Debug.Log ("thereisdata");
			playerMove.playerInput(stringData);
			thereIsData = false;
		}*/
		if(dataToWrite){
			NetworkStream ns2 = server.GetStream();
			Debug.Log ("Data to be wrote");
			if(ns2.CanWrite){
				Debug.Log ("CW");
				if(buffer1full == true){
					Debug.Log ("Buffer1TBW");
					ns2.Write(writeBuffer1,0,writeBuffer1.Length);
					if(buffer2full == true){
						Debug.Log ("Buffer2TBW");
						ns2.Write(writeBuffer2,0,writeBuffer2.Length);
						if(buffer3full == true){
							Debug.Log ("Buffer3TBW");
							ns2.Write(writeBuffer3,0,writeBuffer3.Length);
							if(buffer4full == true){
								ns2.Write(writeBuffer4,0,writeBuffer4.Length);
								buffer4full = false;
							}
							buffer3full = false;
						}
						buffer2full = false;
					}
					buffer1full = false;
				}
				dataToWrite = false;
			}else{
				Debug.Log ("You can not write to the stream.");
			}
		}
	}
	
	public void prepareString(string outputString){
		Debug.Log ("String prep");
		//writeData = Encoding.ASCII.GetBytes(outputString);
		if (buffer1full == true) {
			Debug.Log ("Buffer1Full");
			if(buffer2full == true){
				Debug.Log ("Buffer2Full");
				if(buffer3full == true){
					Debug.Log ("Buffer3Full");
					if(buffer4full == true){
						Debug.Log ("RIP");
					}
					if (buffer4full == false) {
							writeBuffer4 = Encoding.ASCII.GetBytes(outputString);
					}
					buffer4full = true;
				}
				if (buffer3full == false) {
						writeBuffer3 = Encoding.ASCII.GetBytes(outputString);
				}
				buffer3full = true;
			}
			if (buffer2full == false) {
					writeBuffer2 = Encoding.ASCII.GetBytes(outputString);
			}
			buffer2full = true;
		}
		if (buffer1full == false) {
				writeBuffer1 = Encoding.ASCII.GetBytes (outputString);
				}
		buffer1full = true;
		dataToWrite = true;
	}	/*
	private void OpenStream(){
		Debug.Log ("STREAM!");
		NetworkStream ns = server.GetStream ();
		byte[] data = new byte[1024];
		int recv;
		while (runThread == true) {
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
	}*/

	~TCPclient(){
		runThread = false;
	}

	void OnApplicationPause() {
		Debug.Log ("RIPINPEACE");
		writeData = Encoding.ASCII.GetBytes("RIP");
		NetworkStream ns2 = server.GetStream();
		ns2.Write(writeData,0,writeData.Length);
		Application.Quit ();
	}
}
