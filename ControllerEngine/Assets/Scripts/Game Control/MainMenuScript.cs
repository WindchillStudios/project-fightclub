using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour, ISubmitHandler {

	public GameObject logo;
	public GameObject confirmText;
	public Text infoText;

	public AudioClip[] menuSounds;

	Animator logoAnim;
	ParticleSystem[] logoParticles;

	public GameObject[] LevelBackList;

	GameObject[] CharList;
	public GameObject charSelectorFab;
	bool charsLoaded;

	public EventSystem MenuEvents;

	public GameObject levelSelectGui;
	Selectable curLevel;
	bool isSelected;
	string toSubmit;
	float mobiDirectionInput;

	public GameObject charSelectGui;

	public GameObject optionSelectGui;
	
	MatchControl matchController;

	GUI mainMenuGUI;

	Vector3 levelPos = new Vector3(20,0,40);

	enum MenuState{
		MENU_SPLASH,
		MENU_CHARSELECT,
		MENU_LVLSELECT,
		MENU_OPTIONSELECT
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

		GameObject backLev = Instantiate (LevelBackList [randomLevel], levelPos, LevelBackList[randomLevel].transform.rotation) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {

		switch(menu){

		case MenuState.MENU_SPLASH:

			string[] mobiData = matchController.mobileInput;

			if(Input.GetAxis("Jump" + 1) > 0 || Input.GetAxis("Jump" + 2) > 0 || Input.GetAxis("Jump" + 3) > 0 || Input.GetAxis("Jump" + 4) > 0){// || mobiData[0] == "Enter"){

				foreach(ParticleSystem lParts in logoParticles){
					lParts.Stop();
				}

				confirmText.SetActive(false);
				logoAnim.SetBool("isSplash", false);
				this.GetComponent<AudioSource>().PlayOneShot(menuSounds[0]);
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
					confirmText.SetActive(true);

					mobiSubmitChar(i);

					if(Input.GetAxis("Jump" + (i+1)) > 0){
						menu = MenuState.MENU_LVLSELECT;
					}
				}
			}
		
			break;

		case MenuState.MENU_LVLSELECT:

			if(GameObject.FindObjectOfType<TCPclient>() && GameObject.FindObjectOfType<TCPclient>().isOnline){
				GameObject.FindObjectOfType<TCPclient>().prepareString ("Started");
			}

			/* ---- End Character Select ---- */

			charSelectGui.SetActive(false);
			confirmText.SetActive(false);

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

			mobiDirectionInput = mobiLevelSelect();

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

			if(mobiSubmit()){
				menu = MenuState.MENU_OPTIONSELECT;
				isSelected = false;
			}

			break;
		
		case MenuState.MENU_OPTIONSELECT:

			/* ---- End Level Select ---- */
			
			levelSelectGui.SetActive(false);

			/* ---- Start Option Select ---- */
			
			optionSelectGui.SetActive(true);

			if(!isSelected){
				curLevel = Selectable.allSelectables[0];
				curLevel.Select();
				
				isSelected = true;
			}
			
			if(MenuEvents.currentSelectedGameObject){
				curLevel = MenuEvents.currentSelectedGameObject.GetComponent<Selectable>();
			}
			
			mobiDirectionInput = mobiLevelSelect();
			
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
				switch(MenuEvents.currentSelectedGameObject.name){
				case "Survival":
					infoText.text = "5 Life Limit. Make 'em Count";
					GameObject.FindObjectOfType<MatchControl>().setThisGameType(0);
					break;
				case "Timed":
					infoText.text = "Show Your Worth. Clock's Tickin'";
					GameObject.FindObjectOfType<MatchControl>().setThisGameType(1);
					break;
				case "Score":
					infoText.text = "First to 10. Dominate the Competition";
					GameObject.FindObjectOfType<MatchControl>().setThisGameType(2);
					break;
				default:
					break;
				}
			}
			
			if(mobiSubmit()){
				Application.LoadLevel(toSubmit);
			}
			
			break;
		}
	}

	public void OnSubmit(BaseEventData data){

		if (menu == MenuState.MENU_LVLSELECT) {
			menu = MenuState.MENU_OPTIONSELECT;
			isSelected = false;
		}
		else if(menu == MenuState.MENU_OPTIONSELECT){
			Application.LoadLevel (toSubmit);	
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
	
	bool mobiSubmit(){
		
		string[] mobiData = matchController.mobileInput;
		
		if(mobiData.Length > 1){
			if (mobiData [1] == "A") {
				if(mobiData[2] == "Spcl"){
					return true;
				}
				else{
					return false;
				}
			}
			else{
				return false;
			}
		}
		else{
			return false;
		}
	}

	void mobiSubmitChar(int pNum){
		
		string[] mobiData = matchController.mobileInput;
		
		if(mobiData.Length > 1){
			if (mobiData [1] == "A") {
				if(mobiData[2] == "Attk"){

					int mobiPlayer;
					bool result;
					result = int.TryParse(mobiData [0], out mobiPlayer);
					if(!result){
						Debug.Log("Unexpected result is " + mobiData[0]);
					}

					if(mobiPlayer == pNum){
						menu = MenuState.MENU_LVLSELECT;
					}
				}
			}
		}
	}
}
