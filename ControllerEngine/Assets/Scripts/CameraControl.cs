using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public bool isCameraMove;

	GameObject[] players;
	Vector3 camLocation;


	// Use this for initialization
	void Start () {

		if (players == null)
			players = GameObject.FindGameObjectsWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
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
			camLocation.z = -20;

			this.camera.transform.position = camLocation;
		}
		else{
			this.camera.transform.position = new Vector3(0,0,-20);
		}
	}
}
