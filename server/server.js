var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

server.listen(3000);

var coins = [];
var playerSpawnPoints = [];
var clients = [];

app.get('/', function(req, res) {
	res.send('hey you got back get "/"');
});

io.on('connection', function(socket){
    var currentPlayer = {};
    currentPlayer.name = 'unknow';

    socket.on('player connect', function(){
        console.log(currentPlayer.name+' recv: player connect');
        for(var i=0; i<clients.length;i++){
            var playerConnected = {
                name:clients[i].name,
                position:clients[i].position,
                rotation:clients[i].rotation,
                count:clients[i].count
            };   
            socket.emit('other player connected', playerConnected);
            console.log(currentPlayer.name+' emit: other player connected: '+ JSON.stringify(playerConnected));  
        }    
    });

    socket.on('play', function(data){
        console.log(currentPlayer.name+' recv: play: '+JSON.stringify(data));
        if(clients.length === 0){
            numberOfCoins = data.coinSpawnPoints.length;
            coins = [];
            data.coinSpawnPoints.forEach(function(coinSpawnPoints){
                var coin = {
                    name: guid(),
                    position: coinSpawnPoints.position,
                    rotation: coinSpawnPoints.rotation
                };
                coins.push(coin);
            });
            playerSpawnPoints = [];
            data.playerSpawnPoints.forEach(function(_playerSpawnPoint){
                var playSpawnPoint = {
                    position: _playerSpawnPoint.position,
                    rotation: _playerSpawnPoint.rotation
                };
                playSpawnPoints.push(playSpawnPoint);
            });  
        }

        var coinsResponse = {
            coins: coins
        };
        console.log(currentPlayer.name+' emit: coins: '+JSON.stringify(coinsResponse));
        socket.emit('coins', coinsResponse);
        var randomSpawnPoint = playerSpawnPoints[Math.floor(Math.random()* playerSpawnPoints.length)];
        currentPlayer = {
            name:data.name,
            position: randomSpawnPoint.position,
            rotation: playerSpawnPoints.position
        };
        clients.push(currentPlayer);
        console.log(currentPlayer.name+' emit: plat: '+JSON.stringify(currentPlayer));
        socket.broadcastl.emit('other player connected', currentPlayer);
    });

    socket.on('player move',function(data){
        console.log('recv: move: '+JSON.stringify(data));
        currentPlayer.position = data.position;
        socket.broadcast.emit('player move', currentPlayer);
    });

    socket.on('player turn', function(data){
        console.log('recv: turn: '+JSON.stringify(data));
        currentPlayer.rotation = data.rotation;
        socket.broadcast.emit('player turn', currentPlayer);
    });

    socket.on('count',function(data){
        console.log(currentPlayer.name+' rev: count'+JSON.stringify(data));
        if(data.from === currentPlayer.name){
            var indexDamaged = 0;
            coins = coins.map(function(coin, index){
                if(coin.name === data.name){
                    indexDamaged = index;
                    client.coin += data.coinChange;
                }
                return client;
            });

        }

    });

    socket.on('disconnect', function(data){
        console.log(currentPlayer.name+' revc: disconnect '+ JSON.stringify(currentPlayer.name));
        socket.broadcast.emit('other player disconnected',currentPlayer);
        console.log(currentPlayer.name+' bcst: other player disconnected')+JSON.stringify(currentPlayer);
        for(var i=0; i<clients.length; i++){
            if(clients.[i].name === currentPlayer.name){
                clients.splice(i,1);
            }
        }
    });
});

console.log('Server is running.....');

function guid(){
    function s4(){
        return Math.floor((1+Math.random())*0x10000).toString(16).substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-'+ s4() + '-' + s4() + s4() + s4();
};