net = require('net');

var clients = [];
var room1players = [];
var room2players = [];
var room3players = [];
var room4players = [];
var playerID;

net.createServer(function (socket) {
	socket.name = socket.remoteAddress + ":" + socket.remotePort;

	clients.push(socket);

	socket.write("Welcome " + socket.name + "\n");

	broadcast(socket.name + " joined the server\n", socket);

	socket.on('data', function (data) {
		/*var roomcheck = data.split(" ");
		if(roomcheck[0] = "room"){
			switch(roomcheck[1]){
				case "1":
					socket.room = "1";
					room1players.push(socket);
					for(int i=0; i<room1players.length;i++){
						if(room1players[i] == socket){socket.player = i};
					};
					clients.push(socket);
				case "2":
					socket.room = "2";
					room2players.push(socket);
					for(int i=0; i<room2players.length;i++){
						if(room2players[i] == socket){socket.player = i};
					};
					clients.push(socket);
				case "3":
					socket.room = "3";
					room3players.push(socket);
					for(int i=0; i<room2players.length;i++){
						if(room2players[i] == socket){socket.player = i};
					};
					clients.push(socket);
				case "4":
					socket.room = "4";
					room4players.push(socket);
					for(int i=0; i<room2players.length;i++){
						if(room2splayers[i] == socket){socket.player = i};
					};
					clients.push(socket);
				default:{
					console.Log(socket.name+ "has an invalid room");
					break;
				}
			}
		}*/
		broadcast(data, socket);
		console.log("datarecv" + data);
	});

	socket.on('end', function() {
	 clients.splice(clients.indexOf(socket), 1);
	 broadcast(socket.name + " left the server.\n");
	});

	function broadcast(message, sender){
		clients.forEach(function (client) {
			if(client == sender) return;
			client.write(message);
		});
		process.stdout.write(message);
	};
}).listen(8001,'0.0.0.0');