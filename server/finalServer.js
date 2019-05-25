var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

server.listen(3000);


console.log('Server started...');

var coins = [];
var playerSpawnPoints = [];
var clients = [];

app.get('/', function(req, res) {
	res.send('hey you got back get "/"');
});


io.on('connection', function(socket){
    var currentPlayer = {};
	currentPlayer.name = 'unknown';

	socket.on('player connect', function() {
		console.log(currentPlayer.name+' recv: player connect');
		for(var i =0; i<clients.length;i++) {
			var playerConnected = {
				name:clients[i].name,
				position:clients[i].position,
				rotation:clients[i].position,
				point:clients[i].point
			};
			// in your current game, we need to tell you about the other players.
			socket.emit('other player connected', playerConnected);
			console.log(currentPlayer.name+' emit: other player connected: '+JSON.stringify(playerConnected));
		}
	});
    socket.on('play', function(data) {
		console.log(currentPlayer.name+' recv: play: '+JSON.stringify(data));
		// if this is the first person to join the game init the enemies
		if(clients.length === 0) {
			numberOfCoins = data.coinSpawnPoints.length;
			coins = [];
			data.coinSpawnPoints.forEach(function(coinSpawnPoint) {
				var coin = {
					name: guid(),
					position: coinSpawnPoint.position,
					rotation: coinSpawnPoint.rotation,
				};
				coins.push(coin);
			});
			playerSpawnPoints = [];
			data.playerSpawnPoints.forEach(function(_playerSpawnPoint) {
				var playerSpawnPoint = {
					position: _playerSpawnPoint.position,
					rotation: _playerSpawnPoint.rotation
				};
				playerSpawnPoints.push(playerSpawnPoint);
			});
		}

		var coinsResponse = {
			coins: coins
		};
		// we always will send the enemies when the player joins
		console.log(currentPlayer.name+' emit: coins: '+JSON.stringify(coinsResponse));
		socket.emit('coin', coinsResponse);
		var randomSpawnPoint = playerSpawnPoints[Math.floor(Math.random() * playerSpawnPoints.length)];
		currentPlayer = {
			name:data.name,
			position: randomSpawnPoint.position,
			rotation: randomSpawnPoint.rotation,
		};
		clients.push(currentPlayer);
		// in your current game, tell you that you have joined
		console.log(currentPlayer.name+' emit: play: '+JSON.stringify(currentPlayer));
		socket.emit('play', currentPlayer);
		// in your current game, we need to tell the other players about you.
		socket.broadcast.emit('other player connected', currentPlayer);
    });

    socket.on('player move', function(data) {
		console.log('recv: move: '+JSON.stringify(data));
		currentPlayer.position = data.position;
		socket.broadcast.emit('player move', currentPlayer);
	});

    socket.on('disconnect', function() {
        console.log(currentPlayer.name+': logout');
		socket.broadcast.emit('other player disconnected', currentPlayer);
		for(var i=0; i<clients.length; i++) {
			if(clients[i].name === currentPlayer.name) {
				clients.splice(i,1);
			}
		}
	});
});

function guid() {
	function s4() {
		return Math.floor((1+Math.random()) * 0x10000).toString(16).substring(1);
	}
	return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}