using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour, ISubmitHandler {

	public GameObject logo;
	public GameObject text;
	Animator logoAnim;
	ParticleSystem[] logoParticles;

	public GameObject[] LevelBackList;
	GameObject backLev;

	GameObject[] CharList;
	public GameObject charSelectorFab;
	bool charsLoaded;

	public EventSystem MenuEvents;

	public GameObject levelSelectGui;
	Selectable curLevel;
	bool isSelected;
	string toSubmit;

	public GameObject charSelectGui;
	
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

		backLev = Instantiate (LevelBackList [randomLevel], levelPos, this.transform.rotation) as GameObject;
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

			charSelectGui.SetActive(true);

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
					text.SetActive(true);
					if(Input.GetAxis("Jump" + (i+1)) > 0){
						menu = MenuState.MENU_LVLSELECT;
					}
				}
			}
			break;

		case MenuState.MENU_LVLSELECT:

			/* ---- End Character Select ---- */

			charSelectGui.SetActive(false);
			text.SetActive(false);

			foreach(GameObject charSel in CharList){
				charSel.SetActive(false);
			}

			/* ---- Start Level Select ---- */

			levelSelectGui.SetActive(true);
			if(!isSelected){
				curLevel = Selectable.allSelectables[0];
				curLevel.Select();

				isSelected = true;
			}

			if(MenuEvents.currentSelectedGameObject){
				curLevel = MenuEvents.currentSelectedGameObject.GetComponent<Selectable>();
			}

			float mobiDirectionInput = mobiLevelSelect();

			if(mobiDirectionInput > 0){
				if(curLevel.FindSelectableOnRight() != null){
					curLevel = curLevel.FindSelectableOnRight();
					curLevel.Select();
				}
			}
			else if(mobiDirectionInput < 0){
				if(curLevel.FindSelectableOnLeft() != null){
					curLevel = curLevel.FindSelectableOnLeft();
					curLevel.Select();
				}
			}

			if(MenuEvents.currentSelectedGameObject){
				toSubmit = MenuEvents.currentSelectedGameObject.name;
			}

			mobiSubmitLevel(toSubmit);
		
			break;
		}
	}

	public void OnSubmit(BaseEventData data){
		Application.LoadLevel (toSubmit);	
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

	float mobiLevelSelect(){
		string[] mobiData = matchController.mobileInput;

		if(mobiData.Length > 1){
			if (mobiData [1] == "M") {

				bool result;
				float numOutput;

				result = float.TryParse(mobiData [2], out numOutput);
				if(!result){
					Debug.Log("Unexpected result is " + mobiData[2]);
				}

				return numOutput;
			}
			else{
				return 0;
			}
		}
		else{
			return 0;
		}
	}
	
	void mobiSubmitLevel(string levelName){
		
		string[] mobiData = matchController.mobileInput;
		
		if(mobiData.Length > 1){
			if (mobiData [1] == "A") {
				if(mobiData[2] == "Spcl"){
					Application.LoadLevel(levelName);
				}
			}
		}
	}
}
