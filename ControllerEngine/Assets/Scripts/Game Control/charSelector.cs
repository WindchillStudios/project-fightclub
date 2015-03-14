﻿using UnityEngine;
using System.Collections;

public class charSelector : MonoBehaviour {

	public MatchControl matchControl;

	public GameObject AnimControl;

	public int playerNumber;
	public GameObject[] availableChars;
	GameObject curChar;
	int charRotateNum;
	public bool isSelected;

	ParticleSystem partSelect;

	void Start(){
		charRotateNum = 0;
		matchControl = FindObjectOfType<MatchControl>();
		partSelect = this.GetComponentInChildren<ParticleSystem> ();
	}

	void Update(){

		if (!isSelected) {

			if(Input.GetAxis("Horizontal" + playerNumber) > 0){

				if(!AnimControl.GetComponent<Animator>().IsInTransition(0)){
				   charRotateNum ++;
					if(charRotateNum > 4)
					{
						charRotateNum = 0;
					}
				}
			}
			else if(Input.GetAxis("Horizontal" + playerNumber) < 0){
				if(!AnimControl.GetComponent<Animator>().IsInTransition(0)){
					charRotateNum --;
					if(charRotateNum < 0)
					{
						charRotateNum = 4;
					}
				}			
			}

			if(Input.GetAxis("Attack" + playerNumber) > 0){
				selectCharacter();
			}

			AnimControl.GetComponent<Animator> ().SetInteger ("curChar", charRotateNum);
		}
	}
	
	public void selectCharacter(){
		isSelected = true;
		partSelect.Play();
		if(charRotateNum > 0){
			matchControl.players [playerNumber-1] = availableChars [charRotateNum-1];
		}
		else
			isSelected = false;
	}

	public void mobileSet(int setNum){
		charRotateNum = setNum;
	}
}