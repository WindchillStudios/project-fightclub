using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public bool isCameraMove;

	GameObject[] players;
	Vector3 camLocation;
	Vector3 newCamLocation;
	Vector3 lastCamLocation;

	Vector3 cameraDirection;

	GameObject mostLeft;
	GameObject mostRight;

	float fightDistance;

	float yClamp;
	float xClamp;
	float zClamp;

	// Use this for initialization
	void Start () {
		lastCamLocation = new Vector3(0,0,0);

	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (players == null) {
			players = GameObject.FindGameObjectsWithTag ("Player");
		}

		if(mostLeft == null)
			mostLeft = players[0];
		if(mostRight == null)
			mostRight = players[0];

		getCameraPos ();
	}

	void getCameraPos(){
		if(isCameraMove){

			/******Get Horizontal/Vertical******/
					
			foreach (GameObject player in players)
			{
				if(player.activeSelf)
					camLocation += player.transform.position;

				if(player.transform.position.x < mostLeft.transform.position.x)
					mostLeft = player;
				
				if(player.transform.position.x > mostRight.transform.position.x)
					mostRight = player;
			}

			Debug.Log(mostLeft + " " + mostRight);
			
			camLocation = camLocation / players.Length;
			//camLocation = camLocation - lastCamLocation * 0.75f;

			/******Get Depth******/

			fightDistance = mostRight.transform.position.x - mostLeft.transform.position.x;

			camLocation.z = -fightDistance;

			/******Finalize******/

			yClamp = Mathf.Clamp(camLocation.y, -1, 10);
			xClamp = Mathf.Clamp(camLocation.x, -30, 30);
			zClamp = Mathf.Clamp(camLocation.z, -50, -15);


			newCamLocation = new Vector3(xClamp,yClamp,zClamp);

			cameraDirection = newCamLocation - lastCamLocation;

			this.transform.Translate(cameraDirection/10);

			lastCamLocation = this.transform.position;
	
		}
		else{
			this.camera.transform.position = new Vector3(0,5,-35);
		}
	}
}
