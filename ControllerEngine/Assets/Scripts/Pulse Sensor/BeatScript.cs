using UnityEngine;
using System.Collections;

public class BeatScript : MonoBehaviour {

	//public Animator anim;

	void Start(){
		//anim = this.GetComponent<Animator> (); // Here's some animator stuff I didnt get working, if you want to use that instead of resizing talk to me.
		//Debug.Log (this.gameObject);
	}

	void OnEnable(){
		this.gameObject.GetComponent<HeartBeatVisualizer>().OnBeat += increaseSize; //adds a listener for the function in HBVIS script
		this.gameObject.GetComponent<HeartBeatVisualizer>().OffBeat += decreaseSize;
}
	
	
	void OnDisable(){
		this.gameObject.GetComponent<HeartBeatVisualizer>().OnBeat -= increaseSize; //removes the listener, always do this to avoid memory leaks***
		this.gameObject.GetComponent<HeartBeatVisualizer>().OffBeat -= decreaseSize;
	}

	void increaseSize(){
		this.transform.localScale += new Vector3(0.5F, 0.5F, 0.0F); //changes the size of the object - change the scale values as necessary
		//Debug.Log (this.gameObject.name + " Increase"); //Might want to keep this debug for when you want to make sure things are resizing properly
		//anim.Play ("BeatAnimation");
	}

	void decreaseSize(){
		this.transform.localScale += new Vector3(-0.5F, -0.5F, 0.0F); //shrinks it back down again
		//Debug.Log (this.gameObject.name + " Decrease");
	}
}