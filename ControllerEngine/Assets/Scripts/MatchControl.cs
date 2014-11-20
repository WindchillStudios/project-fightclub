using UnityEngine;
using System.Collections;

public class MatchControl : MonoBehaviour {

	int p1Heart;
	int p2Heart;
	int p3Heart;
	int p4Heart;

	int[] playerType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		getAvHeart ();
	
	}

	void SpawnPlayers(){
		foreach (int player in playerType) {
			createPlayer(player);
		}
	}

	void createPlayer(int type)
	{
		switch(type)
		{
			case 0:
				break;
			case 1:
				break;
			case 2:
				break;
			case 3:
				break;
			default:
				break;
		}
	}

	void SpawnEvent(){
	}

	void getAvHeart(){
	}
}
