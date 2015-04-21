using UnityEngine;
using System.Collections;

public class charSelector : MonoBehaviour {

	public MatchControl matchControl;

	public GameObject AnimControl;

	public AudioClip[] selectAudio;

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

			if(Input.GetAxis("Horizontal" + playerNumber) > 0.1f){

				if(!AnimControl.GetComponent<Animator>().IsInTransition(0)){

					this.GetComponent<AudioSource>().Stop(); 

					charRotateNum ++;
					if(charRotateNum > 4)
					{
						charRotateNum = 0;
					}
				}
			}
			else if(Input.GetAxis("Horizontal" + playerNumber) < -0.1f){
				if(!AnimControl.GetComponent<Animator>().IsInTransition(0)){

					this.GetComponent<AudioSource>().Stop(); 
					
					charRotateNum --;
					if(charRotateNum < 0)
					{
						charRotateNum = 4;
					}
				}			
			}

			if(Input.GetAxis("SpecialAttack" + playerNumber) > 0){
				selectCharacter();
			}

			AnimControl.GetComponent<Animator> ().SetInteger ("curChar", charRotateNum);
		}
	}
	
	public void selectCharacter(){

		if(charRotateNum > 0){
			isSelected = true;
			partSelect.Play();
			matchControl.players [playerNumber-1] = availableChars [charRotateNum-1];

			switch (charRotateNum) {
			case 1:
				this.GetComponent<AudioSource>().PlayOneShot(selectAudio[0]); 
				break;
			case 2:
				this.GetComponent<AudioSource>().PlayOneShot(selectAudio[1]); 
				break;
			case 3:
				this.GetComponent<AudioSource>().PlayOneShot(selectAudio[2]); 
				break;
			case 4:
				this.GetComponent<AudioSource>().PlayOneShot(selectAudio[3]); 
				break;
			default:
				break;
			}
		}
		else
			isSelected = false;
	}

	public void mobileSet(int setNum){
		charRotateNum = setNum;
	}
}
