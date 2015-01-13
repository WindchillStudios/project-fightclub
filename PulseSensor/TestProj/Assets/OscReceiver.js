// Make sure you have UDPPacketIO.cs and Osc.cs in the standard assets folder

var RemoteIP : String = "127.0.0.1";
var SendToPort : int = 57131;
var ListenerPort : int = 57130;

//var controller : Transform; 
var handler : Osc;
var textOutput : GUIText;
var spawnObj : Transform;


// Messages are accessible from this instance in other scripts
static var message;
//static var message : Array = [0];


public function Start ()
{	
	// Set up OSC connection
	var udp : UDPPacketIO = GetComponent("UDPPacketIO");
	udp.init(RemoteIP, SendToPort, ListenerPort);
	handler = GetComponent("Osc");
	handler.init(udp);
	
	// Listen to the channels set in the Processing sketch
	handler.SetAddressHandler("/bpmNum", ListenEvent);
	//handler.SetAddressHandler("/pulseTrigger", ListenEvent);
	Debug.Log("Start");
	
}

function Update (){
	textOutput.text = "BPM: " + message;
	//Debug.Log("Update");
	if(message >= 100){
		Debug.Log("Elevated");
		Instantiate(spawnObj, Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity);
	}
	else if (message <= 60){
		Debug.Log("Low");
	}
	else{
		Debug.Log("Normal");
	}
	
//	if(message[1] > 0){
//		Debug.Log("Beat");
//		
//	}
}

public function ListenEvent(oscMessage : OscMessage) : void
{	
//	switch(oscMessage.Address){
//		case "/bpmNum":	i = 0; break;
//		case "/pulseTrigger":	i = 1; break;
//	}
	
	// Make the data available 
	message = oscMessage.Values[0];
	Debug.Log("BPM" + message);
	
	//essage[i] = oscMessage.Values[0];
	//Debug.Log("BPM" + message[0]);
	//Debug.Log("Pulse" + message[1]);
	
} 

