net = require('net');
var util = require('util');

var clients = [];
var room1players = [0,0,0,0];
var room2players = [0,0,0,0];
var room3players = [0,0,0,0];
var room4players = [0,0,0,0];
var gameLobbies = [0,0,0,0,0];
var room1open = true;
var room2open = true;
var room3open = true;
var room4open = true;
var room1full = 0;
var room2full = 0;
var room3full = 0;
var room4full = 0;
var playerID;
var movecheck;

net.createServer(function (socket) {

	socket.name = socket.remoteAddress + ":" + socket.remotePort;

	clients.push(socket);

	socket.write("Welcome " + socket.name);

	socket.on('data', function (data) {
		var dataString = data.toString();
		var stringcheck = dataString.split(" ");
		//.............ROOM SELECTION...............
		if(stringcheck[0] == "Room"){
			console.log("Room is in");
			var switchstring = stringcheck[1];
			console.log(switchstring);
			switch(switchstring){
				case "1":
					open1:
					if(room1open == false){
						console.log("In room false check");
						roomCheck("Progress", socket);
						break open1;
					}
					full1:
					for(r=0;r<room1players.length;r++){
						if(room1full == 4){
							roomCheck("Full", socket);
							break full1;
						}else{
							if(room1players[r] == 0){
								room1players[r] = socket;
								socket.player = r;
								socket.room = "0";
								console.log("Welcome to room 1 " + socket.name);
								//console.log(room1players);
								broadcast("Enter", socket, socket.room);
								roomCheck("Open", socket);
								room1full++;
								break;
							}
							console.log("Counter " + room1full);
						}
					}
					break;
				case "2":
					open2:
					if(room2open == false){
						console.log("In room false check");
						roomCheck("Progress", socket);
						break open2;
					}
					full2:
					for(r=0;r<room2players.length;r++){
						if(room2full == 4){
							roomCheck("Full", socket);
							break full2;
						}else{
							if(room2players[r] == 0){
								room2players[r] = socket;
								socket.player = r;
								socket.room = "1";
								console.log("Welcome to room 2 " + socket.name);
								//console.log(room1players);
								broadcast("Enter", socket, socket.room);
								roomCheck("Open", socket);
								room2full++;
								break;
							}
							console.log("Counter " + room2full);
						}
					}
					break;
				case "3":
					open3:
					if(room3open == false){
						console.log("In room false check");
						roomCheck("Progress", socket);
						break open3;
					}
					full3:
					for(r=0;r<room3players.length;r++){
						if(room3full == 4){
							roomCheck("Full", socket);
							break full3;
						}else{
							if(room3players[r] == 0){
								room3players[r] = socket;
								socket.player = r;
								socket.room = "2";
								console.log("Welcome to room 3 " + socket.name);
								//console.log(room1players);
								broadcast("Enter", socket, socket.room);
								roomCheck("Open", socket);
								room3full++;
								break;
							}
							console.log("Counter " + room3full);
						}
					}
					break;
				case "4":
					open4:
					if(room4open == false){
						console.log("In room false check");
						roomCheck("Progress", socket);
						break open4;
					}
					full4:
					for(r=0;r<room4players.length;r++){
						if(room4full == 4){
							roomCheck("Full", socket);
							break full4;
						}else{
							if(room4players[r] == 0){
								room4players[r] = socket;
								socket.player = r;
								socket.room = "3";
								console.log("Welcome to room 4 " + socket.name);
								//console.log(room1players);
								broadcast("Enter", socket, socket.room);
								roomCheck("Open", socket);
								room4full++;
								break;
							}
							console.log("Counter " + room4full);
						}
					}
					break;
				default:
					console.log(socket.name+ "has an invalid room");
					break;
			}
		}

		//.......MOVE INPUT..............
		if(stringcheck[0] == "Move"){
			broadcast(stringcheck[1], socket.player, socket.room);
		}

		//.......CHARACTER SELECT........
		if(stringcheck[0] == "Char"){
			broadcast(stringcheck[1], socket.player, socket.room);
		}

		//.......CLOSE INSTANCES........
		if(stringcheck[0] == "Close"){
			var switchstring = stringcheck[1];
			switch(switchstring){
				case "Game":
					//Game fully closed out.
					var lobby = socket.lobby;
					toPlayers("Close", socket.lobby);
					gameLobbies.splice(gameLobbies.indexOf(socket),1,"0");
					console.log(gameLobbies);
					switch(lobby){
						case 0:
							room1open = true;
							room1full = 0;
							break;
						case 1:
							room2open = true;
							room2full = 0;
							break;
						case 2:
							room3open = true;
							room3full = 0;
							break;
						case 3:
							room4open = true;
							room4full = 0;
							break;
						default:
							break;
					}
					socket.destroy();
					break;
				case "Menu":
					//Game closed to menu.
					console.log("Close to menu");
					var lobby = socket.lobby;
					switch(lobby){
						case 0:
							room1open = true;
							room1full = 0;
							break;
						case 1:
							room2open = true;
							room2full = 0;
							break;
						case 2:
							room3open = true;
							room3full = 0;
							break;
						case 3:
							room4open = true;
							room4full = 0;
							break;
						default:
							break;
					}
					toPlayers("Close", socket.lobby);
					break;
				case "Over":
					//Game "closes" due to someone winning the game.
					var winner = stringcheck[2];
					var lobby = socket.lobby;
					gameOver(winner, socket.lobby);
				default:
					break;
			}
		}
		//........PLAYER LEFT GAME..........
		if(data.toString() == "RIP"){
			var RIPplayer = socket.room;
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

		//......GAME STARTED. SET BOOLEAN TO FALSE TO AVOID ENTRY ........
		if(data.toString() == "Started"){
			var lobby = socket.lobby;
			console.log(lobby);
			switch(lobby){
				case 0:
					console.log("Set to room1 false");
					room1open = false;
					break;
				case 1:
					room2open = false;
					break;
				case 2:
					room3open = false;
					break;
				case 3:
					room4open = false;
					break;
				default:
					break;
			}
		}

		//..........NEW GAME INSTANCE JOINED SERVER...........
		if(data.toString() == "Game"){
			for(g=0;g<gameLobbies.length;g++){
				if(gameLobbies[g] == 0){
					gameLobbies[g] = socket;
					socket.lobby = g;
					console.log(gameLobbies);
					break;
				}
			}
		}
	});

	socket.on('error', function(err){
		console.log("Errored" + err);
		console.error(err);
	});

	socket.on('end', function() {
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
	 gameLobbies.splice(gameLobbies.indexOf(socket));
	});

	//............SEND INPUT TO APPROPRIATE GAME LOBBY.........
	function broadcast(message, sender, room){
		gameLobbies.forEach(function (game){
			if(game.lobby == room){
				if(message == "Enter"){
					game.write(message);

				}else{
					game.write(sender + ":" + message);	
				}			
			}
		});
		console.log("To room " + room + " message: " + message);
	};

	//..........SEND INPUT TO PLAYERS FROM THEIR GAME LOBBY..........
	function toPlayers(message, room){
		var gamelobby = room;
		switch(gamelobby){
			case 0:
				room1players.forEach(function (player){
					if(player != 0){
						console.log("r1p");
						player.write(message);
						room1players.splice(room1players.indexOf(player), 1, "0");
					}
				});
				break;
			case 1:
				room2players.forEach(function (player){
					if(player != 0){
						player.write(message);
						room2players.splice(room2players.indexOf(player), 1, "0");
					}
				});
				break;
			case 2:
				room3players.forEach(function (player){
					if(player != 0){
						player.write(message);
						room3players.splice(room3players.indexOf(player), 1, "0");
					}
				});
				break;
			case 3:
				room4players.forEach(function (player){
					if(player != 0){
						player.write(message);
						room4players.splice(room4players.indexOf(player), 1, "0");
					}
				});
				break;
			default:
				console.log(socket.name+ "has an invalid room");
				break;
		}
	};

	//.............SEND GAME WON OR LOSS MESSAGE TO PLAYERS POST-GAME............
	function gameOver(winner, room){
		var gamelobby = room;
		console.log("WINNER VARIABLE = " + winner);
		switch(gamelobby){
			case 0:
				for(i=0; i<room1players.length;i++){
					if(i!=winner){
						console.log("Loser Player " + i);
						if(room1players[i] != 0){
							room1players[i].write("Loss");
						}
					}else if(i == winner){
						console.log("Winner Player " + i);
						room1players[i].write("Win");
					}
				}
				break;
			case 1:
				for(i=0; i<room2players.length;i++){
					if(i!=winner){
						console.log("Loser Player " + i);
						if(room2players[i] != 0){
							room2players[i].write("Loss");
						}
					}else if(i == winner){
						console.log("Winner Player " + i);
						room2players[i].write("Win");
					}
				}
				break;
			case 2:
				for(i=0; i<room3players.length;i++){
					if(i!=winner){
						console.log("Loser Player " + i);
						if(room3players[i] != 0){
							room3players[i].write("Loss");
						}
					}else if(i == winner){
						console.log("Winner Player " + i);
						room3players[i].write("Win");
					}
				}
				break;
			case 3:
				for(i=0; i<room4players.length;i++){
					if(i!=winner){
						console.log("Loser Player " + i);
						if(room4players[i] != 0){
							room4players[i].write("Loss");
						}
					}else if(i == winner){
						console.log("Winner Player " + i);
						room4players[i].write("Win");
					}
				}
				break;
			default:
				console.log(socket.name+ "has an invalid room");
				break;
		}
	}

	//...........NOTIFIES PLAYER IF ROOM IS FULL OR IN PROGRESS.............
	function roomCheck(message, user){
		console.log("Roomcheck message " + message);
		user.write(message);
	}

}).listen(8001,'0.0.0.0');