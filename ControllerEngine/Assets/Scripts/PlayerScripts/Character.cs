using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	public int playerNumber;
	public int gameType;

	public float maxHealth;
	public float maxSpeed;
	public float maxJump;
	
	public enum State
	{
		STATE_FROZEN,
		STATE_IDLE,
		STATE_RUNNING,
		STATE_JUMPING,
		STATE_FALLING,
		STATE_ATTACKING,
		STATE_KNOCKBACK,
		STATE_DASH
	}

	public enum lifeState 
	{
		STATE_ALIVE,
		STATE_DEAD
	}

	public State state_;
	public lifeState lifeState_;

	/******Input*********/

	float moviInput;
	float moveInput;
	public string actionInput;

	/******Heatlh*********/

	float health;
	int invTimer;
	float respawnTimer;
	
	bool isHurt;
	bool canRespawn;
	int lives = 5;
	int deaths;
	int kills;

	/*************Attacking***********/

	public bool canAttack;
	public bool canListen;
	public bool isListening;

	/************Movement*************/

	public Animator model;
	CharacterController body;
	public Renderer[] skin;
	public attacks charAttacks;
	public bool canMove;

	//Knockback
	int knockTimer;
	Vector3 knockForce;
	
	//Dashning
	bool canDash;
	float dashTimer = 0.0f;
	float dashCool = 0.0f;
	
	//Horizontal
	float currentSpeed = 0.0f;
	float acceleration = 5.0f;
	public float facing;
	float horizontalMove;
	
	//Vertical
	float currentJump;
	float jumpAcceleration;

	//External
	Vector2 extForce;

	//Depth
	float depthCorrection;
	
	public float maxGravity;
	float gravity;
	float verticalMove;
	bool canJump = true;

	bool onRSlope = false;
	bool onLSlope = false;

	Vector3 movement = new Vector3 (0,0,0);

	/************* Start ******************/

	public void Start()
	{
		body = this.GetComponent<CharacterController> ();
		model = this.GetComponentInChildren<Animator>();
		skin = this.GetComponentsInChildren<Renderer>();
		charAttacks = this.GetComponentInChildren<attacks> ();

		health = maxHealth;
		canMove = true;

		lifeState_ = lifeState.STATE_ALIVE; 

		maxGravity = -20.0f;
		gravity = maxGravity;
		canDash = true;
		canListen = true;
	}


	/************* Update ******************/

	void FixedUpdate () {

		checkIdleReset ();

		moviInput = moviInput / 50;

		updateMovement ();

		if( lifeState_ == lifeState.STATE_ALIVE)
		{
			updateHealth ();
			updateAttack ();
		}

		handleCharState ();

		movement.x = horizontalMove * Time.deltaTime;
		movement.y = verticalMove * Time.deltaTime;
		movement.z = depthCorrection * Time.deltaTime;
		
		body.Move (movement);
	}

	/************* Read Controller Input ******************/

	public void readInput(string type, string conInput){

		bool result;

		if(type == "M"){
			result = float.TryParse(conInput, out moviInput);
			if(!result){
				Debug.Log("Unexpected result is " + conInput);
			}
		}
		else if(type == "A"){
			actionInput = conInput;
		}
	}

	void checkIdleReset(){
		if(model.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.Idle") && state_ != State.STATE_FROZEN){
			model.SetBool("isCombo", false);
			if(state_ != State.STATE_JUMPING){
				state_ = State.STATE_IDLE;
			}
		}
	}

	void handleCharState()
	{
		switch(state_)
		{

		case State.STATE_FROZEN:

			canMove = false;
			canAttack = false;
			canJump = false;

			if(Application.loadedLevelName != "Results"){
				if(gravity > maxGravity){
					gravity -= 2.0f;
				}
				verticalMove = gravity;
			}else{
				verticalMove = 0.0f;
			}

			model.SetInteger("state",0);

			break;

		/* --------- IDLE ---------- */

		case State.STATE_IDLE:

			if(body.isGrounded){
				if (moveInput == 0)
					model.SetInteger("state", 0);
				else
					model.SetInteger("state", 1);
			}

			canAttack = true;
			canMove = true;

			if(body.isGrounded)
			{
				canJump = true;
			}

			break;

		/* --------- JUMPING ---------- */

		case State.STATE_JUMPING:
			model.SetInteger("state", 2);

			canAttack = true;
			canMove = true;

			canJump = false;
			gravity = 0.0f;
			
			currentJump = jumpAcceleration;
			if(jumpAcceleration > 0)
			{
				jumpAcceleration -= 1.0f;
			}
			else 
			{
				state_ = State.STATE_IDLE;
			}
			verticalMove = currentJump;

			break;

		/* --------- FALLING ---------- */
		
		case State.STATE_FALLING:
			model.SetInteger("state", 3);

			canJump = false;
			canMove = true;
			canAttack = true;

			if(gravity > maxGravity){
				gravity -= 2.0f;
			}
			verticalMove = gravity;

			if(body.isGrounded){

				model.SetInteger("state", 0);

				state_ = State.STATE_IDLE;
			}

			break;
		
		/* --------- ATTACK ---------- */
			
		case State.STATE_ATTACKING:

			canMove = false;
			canAttack = false;

			verticalMove = -10;

			model.SetInteger("state", 6);

			if(model.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
			{
				if(!model.GetNextAnimatorStateInfo(0).IsTag("Attack")){
					if(model.IsInTransition(0)){
						state_ = State.STATE_IDLE;
					}
				}

				if(model.IsInTransition(0)){
					canListen = false;


				}
				else{
					canListen = true;
				}

				if(canListen){
					if(Input.GetAxis("Attack" + playerNumber) > 0 || actionInput == "Attk")
					{
						isListening = true;

						if(isListening){
							DoAttack(false, 4);
							isListening = false;
						}

					}
				}
			}

			break;

		/* --------- DASH ---------- */

		case State.STATE_DASH:

			model.SetInteger("state", 1);

			canAttack = false;
			canMove = false;

			canDash = false;
			
			if(dashTimer < 3){
				dashTimer ++;
				Vector3 dashMove = new Vector3(facing,0,0);
				body.Move(dashMove*1.0f);
			}
			else
				state_ = State.STATE_IDLE;
			
			break;

		/* --------- KNOCKBACK ---------- */

		case State.STATE_KNOCKBACK:

			model.SetInteger("state", 4);

			canAttack = false;
			canMove = false;
			
			if(knockForce != Vector3.zero)
			{					
				if(knockForce.y > 0)
				{
					knockForce.y -= 0.5f;
				}
				else
				{					
					knockForce = Vector3.zero;
				}
			}
			else
				state_ = State.STATE_IDLE;
				
			break;
		}

		switch(lifeState_)
		{
		
		/* --------- ALIVE ---------- */
		
		case lifeState.STATE_ALIVE:
			model.SetBool("isDead", false);
			break;
		
		/* --------- DEAD ---------- */
		
		case lifeState.STATE_DEAD:

			model.SetBool("isDead", true);
			model.SetInteger("state", 5);

			canAttack = false;
			canMove = false;

			checkRespawn();

			if(canRespawn){
				respawnTimer += (1 * Time.deltaTime);
				if(respawnTimer > 5)
				{
					respawnTimer = 0;
					respawn();
				}
			}
			else{
				this.gameObject.SetActive(false);
			}
			
			break;
				
		}
	}



	/* -----------------------------------------------------------*/

	/***********   | ----- |   FUNCTIONS   | ----- |   ************/

	/* -----------------------------------------------------------*/


	void updateMovement()
	{
		if(canMove)
		{
			facing = Mathf.Sin(this.transform.eulerAngles.y*Mathf.Deg2Rad);

			getInputMovement ();
			
			if(!onRSlope && !onLSlope)
				horizontalMove = currentSpeed * moveInput;

			if(state_ != State.STATE_KNOCKBACK){
				knockForce = Vector3.zero;
			}
		}
		else
		{
			horizontalMove = 0;
		}
		
		if(body.transform.position.z > 0)
		{
			depthCorrection = -0.1f;
		}
		else if(body.transform.position.z < 0)
		{
			depthCorrection = 0.1f;
		}

		horizontalMove += extForce.x;
		verticalMove += extForce.y;

		extForce = Vector2.zero;

		horizontalMove += knockForce.x;
		verticalMove += knockForce.y;
		
		if(onRSlope)
			horizontalMove = -7.0f;
		else if(onLSlope)
			horizontalMove = 7.0f;
		
		/* ---------- FALLING ---------- */
		
		if (state_ == State.STATE_IDLE && !body.isGrounded) {
			
			state_ = State.STATE_FALLING;
		}

		if(state_ != State.STATE_JUMPING)
		{
			currentJump = 0.0f;
			jumpAcceleration = maxJump;
		}
	}

	void updateHealth()
	{
		if(isHurt)
		{
			foreach(Renderer mat in skin)
			{
				mat.material.color = Color.red;
			}
			invTimer ++;
			
			if(invTimer >= 10){
				isHurt = false;
				invTimer = 0;
			}
		}
		else
		{
			foreach(Renderer mat in skin)
			{
				mat.material.color = Color.white;
			}
		}
	}

	void updateAttack(){

		updateSpecial ();

		int attDirection;

		/* ------ GET DIRECTION INPUTS -----*/
		
		if (moveInput != 0)
		{
			attDirection = 1;
		}
		else if (moveInput > 0)
		{
			attDirection = 2;
		}
		else if (moveInput < 0)
		{
			attDirection = 3;
		}
		else
			attDirection = 0;

		/* ----- DO ATTACKS ----- */

		if(canAttack)
		{
			if(actionInput == "Attk" || Input.GetAxis("Attack" + playerNumber) > 0)
			{
				state_ = State.STATE_ATTACKING;
				DoAttack(false, attDirection);
			}
			else if(actionInput == "Spcl" || Input.GetAxis("SpecialAttack" + playerNumber) > 0)
			{
				state_ = State.STATE_ATTACKING;
				DoAttack(true, attDirection);
			}
		}
	}

	public void takeDamage(float damage, GameObject attacker){

		if(lifeState_ != lifeState.STATE_DEAD)
		{
			if(!isHurt){
				if(damage > 0)
				{
					health = health - damage;
					isHurt = true;
					
					if(health < 1)
					{
						if(lifeState_ != lifeState.STATE_DEAD)
							takeDeath(attacker);
					}
				}
			}
		}
	}
	
	void takeDeath(GameObject attacker)
	{
		if(attacker.GetComponent<Character>()){
			attacker.GetComponent<Character> ().addKill ();
		}

		lifeState_ = lifeState.STATE_DEAD;
		deaths ++;
		lives--;
	}

	public void addKill(){
		kills++;
	}
	public int readKills(){
		return kills;
	}
	public int readLives(){
		return lives;
	}

	void checkRespawn(){

		switch(gameType){
		
		case 0: // Survival
			if(lives > 0){
				canRespawn = true;
			}
			else{
				canRespawn = false;
			}
			break;

		case 1: // TDM Timed
			canRespawn = true;
			break;
		
		case 2: // TDM Score
			canRespawn = true;
			break;
		
		default: // Default
			canRespawn = true;
			break;
		}
	}

	public bool getCanRespawn(){
		return canRespawn;
	}

	public void setLives(int i){
		lives = i;
	}

	public void affectGravity(float grav){
		gravity += grav;
	}

	void respawn(){

		GameObject[] spawnLocs = GameObject.FindGameObjectsWithTag("Respawn");
		int pickSpawn = Random.Range (0, spawnLocs.Length);

		spawnLocs[pickSpawn].GetComponentInChildren<ParticleSystem>().Play();

		this.gameObject.transform.position = spawnLocs [pickSpawn].transform.position;
		lifeState_ = lifeState.STATE_ALIVE;

		health = maxHealth;
		isHurt = false;
	}

	public float getHealth(){
		return health;
	}

	public void setGameType(int gameTypeNum){
		gameType = gameTypeNum;
	}

	/* ------- FINDING KNOCKBACK ------- */

	public void knockBack(Vector3 force)
	{
		if(lifeState_ != lifeState.STATE_DEAD)
		{
			knockForce = force;
			state_ = State.STATE_KNOCKBACK;
		}
	}

	public void extMovement(Vector3 force)
	{
		extForce = new Vector2(force.x, force.y);
	}

	/* ------- FINDING INPUTS ------- */

	void getInputMovement(){

		// HORIZONTAL ///

		if(Input.GetAxis("Horizontal" + playerNumber) != 0){
			moveInput = Input.GetAxis ("Horizontal" + playerNumber);
		}
		else if(moviInput != 0){
			moveInput = moviInput;
		}
		else
			moveInput = 0;


		if(moveInput != 0)
		{
			if(moveInput > 0){
				body.transform.eulerAngles = new Vector3 (0, 90, 0);
			}
			else if(moveInput < 0){
				body.transform.eulerAngles = new Vector3 (0, 270, 0);
			}
			
			if (currentSpeed < maxSpeed) {
				currentSpeed += acceleration;
			}
		}
		else{
			currentSpeed = 0;
		}

		// DASHING ///
		if(actionInput == "DshL" || Input.GetAxis("Dash"  + playerNumber) > 0)
		{
			if(canDash)
			{
				dashCool = 0.0f;
				dashTimer = 0.0f;
				state_ = State.STATE_DASH;
			}
			actionInput = "idle";
		}

		if(!canDash){
			
			if(dashCool < 100)
			{
				dashCool ++;
			}
			else
			{
				canDash = true;
			}
		}
		
		// VERTICAL ///

		if (actionInput == "Jump" || Input.GetAxis ("Jump"  + playerNumber) > 0) {
			if(canJump){
				state_ = State.STATE_JUMPING;
			}
			actionInput = "idle";
		}
	}

	public virtual void DoAttack(bool isSpecial, int attDirection){
	}
	
	public virtual void updateSpecial(){
	}

	//SOME COLLISION DETECTION//
	void OnTriggerEnter(Collider hit){
		
		// INSTANT DEATH OBJECT ////////////
		
		if (hit.gameObject.tag == "Death") { 
			takeDamage(1000.0f, hit.gameObject);
		}
	}

	public void setFrozen(bool isFrozen){
		if (isFrozen) {
			state_ = State.STATE_FROZEN;
		}
		else{
			state_ = State.STATE_IDLE;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit){
		
		// PHYSICS OBJECT INTERACTION ////////
		
		if(hit.gameObject.rigidbody)
		{
			hit.rigidbody.AddForceAtPosition(hit.moveDirection*10.0f, hit.point);
		}
		
		// INSTANT DEATH OBJECT ////////////
		
		if (hit.gameObject.tag == "Death") { 
			takeDamage(1000.0f, hit.gameObject);
		}

		// JUMP CANCEL COLLISION ////////////

		if (hit.moveDirection.y == 1.0f) {
				state_ = State.STATE_FALLING;
		}

		// TRAP INTERACTION ///////////////
		
		if(hit.gameObject.tag == "Trap")
		{
			if(hit.gameObject.GetComponent<TrapScript>())
			{
				TrapScript trap = hit.gameObject.GetComponent<TrapScript>();
				if(trap.isActive == true)
				{
					trap.doTrap(this.gameObject);
				}
			}
		}
		
		// SLOPE CORRECTION ////////////////
		
		if(hit.moveDirection.y < -0.5f)
		{
			if(hit != null)
			{
				float slope = (hit.normal.y/hit.normal.x);
				
				if(slope < 1 && slope > 0){
					onLSlope = true;
				}
				else{
					onLSlope = false;
				}
				
				if(slope < 0 && slope > -1){
					onRSlope = true;
				}
				else{
					onRSlope = false;
				}
			}
			else{
				onRSlope = false;
				onLSlope = false;
			}
		}
	}
}
