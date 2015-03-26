net = require('net');

var clients = [];
var room1players = [0,0,0,0];
var room2players = [0,0,0,0];
var room3players = [0,0,0,0];
var room4players = [0,0,0,0];
var gameLobbies = [];
var playerID;
var movecheck;

net.createServer(function (socket) {
	socket.name = socket.remoteAddress + ":" + socket.remotePort;

	clients.push(socket);

	socket.write("Welcome " + socket.name + "\n");

	socket.on('data', function (data) {
		var dataString = data.toString();
		var stringcheck = dataString.split(" ");
		//console.log("MOVECHECK" + movecheck[0]);
		if(stringcheck[0] == "Room"){
			console.log("Room is in");
			var switchstring = stringcheck[1];
			console.log(switchstring);
			switch(switchstring){
				case "1":
					console.log("here");
					for(r=0;r<room1players.length;r++){
						if(room1players[r] == 0){
							room1players[r] = socket;
							socket.player = r;
							socket.room = "0";
							console.log(socket);
							break;
						}
					}
					break;
				case "2":
					for(r=0;r<room2players.length;r++){
						if(room2players[r] == 0){
							room2players[r] = socket;
							socket.player = r;
							socket.room = "1";
							console.log(socket);
							break;
						}
					}
					/*if(room2players.length == 4){
						roomCheck(socket);
						break;
					}
					socket.room = "1";
					room2players.push(socket);
					for(i=0; i<room2players.length;i++){
						if(room2players[i] == socket){socket.player = i};
					};
					console.log(socket);
					clients.push(socket);*/
					break;
				case "3":
					for(r=0;r<room3players.length;r++){
						if(room3players[r] == 0){
							room3players[r] = socket;
							socket.player = r;
							socket.room = "2";
							console.log(socket);
							break;
						}
					}
					break;
				case "4":
					for(r=0;r<room4players.length;r++){
						if(room4players[r] == 0){
							room4players[r] = socket;
							socket.player = r;
							socket.room = "3";
							console.log(socket);
							break;
						}
					}
					break;
				default:{
					console.log(socket.name+ "has an invalid room");
					break;
				}
			}
		}

		if(stringcheck[0] == "Move"){
			broadcast(stringcheck[1], socket.player, socket.room);
		}

		if(stringcheck[0] == "Char"){
			broadcast(stringcheck[1], socket.player, socket.room);
		}

		if(data.toString() == "RIP"){
			var RIPplayer = socket.room;
			console.log("RIPRIPRIPRIP");
			switch(RIPplayer){
				case "0":
					room1players.splice(room1players.indexOf(socket), 1, "0");
					console.log("Player left room 1");
					break;
				case "1":
					room2players.splice(room2players.indexOf(socket), 1, "0");
					console.log("Player left room 2");
					break;
				case "2":
					room3players.splice(room3players.indexOf(socket), 1, "0");
					console.log("Player left room 3");
					break;
				case "3":
					room4players.splice(room4players.indexOf(socket), 1, "0");
					console.log("Player left room 4");
					break;
				default:
					break;
			}
			socket.destroy();
		}
		
		if(data.toString() == "Game"){
			console.log("GAME");
			gameLobbies.push(socket);
			if(gameLobbies.length >= 1){
				console.log("Game pushed");
				for(i=0; i<gameLobbies.length;i++){
							if(gameLobbies[i] == socket){socket.lobby = i};
							break;
				};
				console.log(gameLobbies);
			}
		}
		//broadcast(data, socket, socket.room);
		//console.log("datarecv" + data);
	});

	socket.on('error', function(err){
		console.log("Errored" + err);
		console.error(err);
	});

	socket.on('end', function() {
		console.log("In end screen");
		switch(socket.room){
			case 0:
				room1players.splice(room1players.indexOf(socket), 1, "0");
				console.log("Player left room 1");
				break;
			case 1:
				room2players.splice(room2players.indexOf(socket), 1, "0");
				console.log("Player left room 2");
				break;
			case 2:
				room3players.splice(room3players.indexOf(socket), 1, "0");
				console.log("Player left room 3");
				break;
			case 3:
				room4players.splice(room4players.indexOf(socket), 1, "0");
				console.log("Player left room 4");
				break;
			default:
				break;
		}
	 clients.splice(clients.indexOf(socket), 1, "0");
	 gameLobbies.splice(gameLobbies.indexOf(socket));
	 //broadcast(socket.name + " left the server.\n");
	});

	function broadcast(message, sender, room){
		/*clients.forEach(function (client) {
		if(client == sender) return;
			client.write(message);
		});*/
		gameLobbies.forEach(function (game){
			if(game.lobby == room){
				game.write(sender + ":" + message);				
			}
			/*console.log("game lobby: " + game.lobby);
			console.log("room: " + room);
			console.log("Player: " + sender)*/
		});/*
		realroom = room-1;
		client = gameLobbies[realroom];
		client.write(message);*/
		console.log(message);
	};

	function roomCheck(user){
		user.write("Room is full.");
	}

}).listen(8001,'0.0.0.0');