﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public bool isCameraMove;

	GameObject[] players;
	Vector3 camLocation;
	Vector3 lastCamLocation;


	// Use this for initialization
	void Start () {

		if (players == null)
			players = GameObject.FindGameObjectsWithTag("Player");
	}
	
	// Update is called once per frame
	void LateUpdate () {
		getCameraPos ();
	}

	void getCameraPos(){
		if(isCameraMove){
			camLocation = new Vector3 (0, 0, 0);
		
			foreach (GameObject player in players)
			{
				if(player.activeSelf)
					camLocation += player.transform.position;
			}
			
			camLocation = camLocation / players.Length;
			//camLocation = camLocation - lastCamLocation * 0.75f;
			camLocation.z = -40;


			this.camera.transform.position = camLocation;
			lastCamLocation = camLocation;
		}
		else{
			this.camera.transform.position = new Vector3(0,0,-20);
		}
	}
}
