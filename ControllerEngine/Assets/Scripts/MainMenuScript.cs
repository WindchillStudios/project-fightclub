using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public GameObject[] LevelBackList;
	GameObject[] CharList;
	GUI mainMenuGUI;

	Vector3 levelPos = new Vector3(0,0,40);

	// Use this for initialization
	void Start () {

		int randomLevel = Random.Range (0, (LevelBackList.Length-1));

		GameObject curLev = Instantiate (LevelBackList [randomLevel], levelPos, this.transform.rotation) as GameObject;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
