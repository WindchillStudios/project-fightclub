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

	string input, stringData;
	Queue<string> inputs = new Queue<string>();	
	Queue<byte[]> outputs = new Queue<byte[]>();
	bool buffer1full, buffer2full, buffer3full, buffer4full;
	byte[] writeData, writeBuffer1, writeBuffer2, writeBuffer3, writeBuffer4;
	TcpClient server;
	public Movement playerMove;
	bool runThread;
	bool thereIsData = false;
	bool dataToWrite = false;

	void Start(){
		Debug.Log ("Hi");
		server = new TcpClient ("localhost", 8001);
		if (server.Connected) {
			Debug.Log ("Connected");
			runThread = true;
			new Thread (OpenStream).Start();
			prepareString ("Game");
		}
	}

	void Update(){
		if(thereIsData == true){
			Debug.Log ("thereisdata");
			playerMove.playerInput(inputs.Dequeue ());
			thereIsData = false;
		}
		if(dataToWrite){
			NetworkStream ns2 = server.GetStream();
			Debug.Log ("Data to be wrote");
			if(ns2.CanWrite){
				Debug.Log ("CW");
				ns2.Write(outputs.Dequeue(),0,4);
				if(outputs.Count == 0){
					dataToWrite = false;
				}
			}else{
				Debug.Log ("You can not write to the stream.");
			}
		}
	}

	public void prepareString(string outputString){
		Debug.Log ("String prep");
		byte[] encodedString = Encoding.ASCII.GetBytes (outputString);
		outputs.Enqueue(encodedString);
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

	~TCPclient(){
		Debug.Log ("down");
		runThread = false;
	}
}
