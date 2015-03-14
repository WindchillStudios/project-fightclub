using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

	public GameObject logo;
	public GameObject text;
	Animator logoAnim;
	ParticleSystem[] logoParticles;

	public GameObject[] LevelBackList;

	GameObject[] CharList;
	public GameObject charSelectorFab;
	bool charsLoaded;

	public GameObject levelSelect;
	Selectable curLevel;

	MatchControl matchController;

	GUI mainMenuGUI;

	Vector3 levelPos = new Vector3(20,0,40);

	enum MenuState{
		MENU_SPLASH,
		MENU_CHARSELECT,
		MENU_LVLSELECT
	}

	MenuState menu;

	// Use this for initialization
	void Start () {
		matchController = MatchControl.FindObjectOfType<MatchControl> ();
		menu = MenuState.MENU_SPLASH;
		CharList = new GameObject[4];

		logoAnim = logo.GetComponentInChildren<Animator> ();
		logoParticles = logo.GetComponentsInChildren<ParticleSystem>();

		int randomLevel = Random.Range (0, (LevelBackList.Length));

		GameObject curLev = Instantiate (LevelBackList [randomLevel], levelPos, this.transform.rotation) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {

		switch(menu){

		case MenuState.MENU_SPLASH:
			if(Input.GetAxis("Jump1") > 0){

				foreach(ParticleSystem lParts in logoParticles){
					lParts.Stop();
				}

				text.SetActive(false);
				logoAnim.SetBool("isSplash", false);
				menu = MenuState.MENU_CHARSELECT;
			}
			break;

		case MenuState.MENU_CHARSELECT:

			if(!charsLoaded){
				for(int i = 0; i < 4; i++){
					CharList[i] = Instantiate(charSelectorFab, new Vector3(((i*3)+22),16,(2*i-10)), charSelectorFab.transform.rotation) as GameObject;
					CharList[i].GetComponent<charSelector>().playerNumber = i+1;
				}
				charsLoaded = true;
			}

			if(matchController.mobileInput.Length > 2){
				mobiSelect();
			}

			for(int i=0; i < CharList.Length; i++){
				if(CharList[i].GetComponent<charSelector>().isSelected){
					if(Input.GetAxis("Jump" + (i+1)) > 0){
						menu = MenuState.MENU_LVLSELECT;
					}
				}
			}
			break;

		case MenuState.MENU_LVLSELECT:
			foreach(GameObject charSel in CharList){
				charSel.SetActive(false);
			}
			levelSelect.SetActive(true);

			break;
		}
	}

	void mobiSelect(){

		string[] mobiData = matchController.mobileInput;
		int mobiChar;
		int mobiPlayer;
		bool result;

		if(mobiData[1] == "C"){

			result = int.TryParse(mobiData [0], out mobiPlayer);
			if(!result){
				Debug.Log("Unexpected result is " + mobiData[0]);
			}

			if(mobiData[2] != "Confirm"){
				result = int.TryParse(mobiData [2], out mobiChar);
				if(!result){
					Debug.Log("Unexpected result is " + mobiData[2]);
				}

				CharList [mobiPlayer].GetComponent<charSelector> ().mobileSet (mobiChar);
			}
			else if(mobiData[2] == "Confirm"){
				CharList[mobiPlayer].GetComponent<charSelector>().selectCharacter();
				CharList[mobiPlayer].GetComponent<Character>().isMobileControlled = true;
			}
		}
	}
}
