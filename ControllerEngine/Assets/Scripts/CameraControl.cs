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
		players = GameObject.FindGameObjectsWithTag ("Player");
		
		if(mostLeft == null)
			mostLeft = players[0];
		if(mostRight == null)
			mostRight = players[0];

	}
	
	// Update is called once per frame
	void LateUpdate () {
		getCameraPos ();
	}

	void getCameraPos(){
		if(isCameraMove){

			if(players.Length > 1)
			{
				/******Get Horizontal/Vertical******/
						
				foreach (GameObject player in players)
				{
					if(player != null){
						if(player.activeSelf){
							camLocation += player.transform.position;
						}

						if(player.transform.position.x < mostLeft.transform.position.x)
							mostLeft = player;
						
						if(player.transform.position.x > mostRight.transform.position.x)
							mostRight = player;
					}
				}

				camLocation = camLocation / players.Length;
				//camLocation = camLocation - lastCamLocation * 0.75f;

				/******Get Depth******/

				fightDistance = mostRight.transform.position.x - mostLeft.transform.position.x;

				camLocation.z = -fightDistance/1.5f;

				/******Finalize******/

				yClamp = Mathf.Clamp(camLocation.y, -1, 10);
				zClamp = Mathf.Clamp(camLocation.z, -50, -15);

				yClamp += 3;

				newCamLocation = new Vector3(camLocation.x,yClamp,zClamp);

				cameraDirection = newCamLocation - lastCamLocation;

				this.transform.Translate(cameraDirection/10);

				lastCamLocation = this.transform.position;

				camLocation = Vector3.zero;
		
			}
			else{
				if(players.Length == 1){
					this.transform.position = new Vector3(players[0].transform.position.x, players[0].transform.position.y + 2.5f, -10);
				}
			}
		}
		else{
			this.camera.transform.position = new Vector3(0,5,-35);
		}
	}
}
