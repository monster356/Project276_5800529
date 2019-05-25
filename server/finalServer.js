var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

server.listen(3000);

var Player = require('./Classes/Player.js');

console.log('Server started...');

var players = [];
var sockets = [];

io.on('connection', function(socket){
    console.log('clientConnection !!');
    
    var player = new Player();
    var thisPlayerID = player.id;

    players[thisPlayerID] = player;
    sockets[thisPlayerID] = socket;

    socket.emit('register', {id: thisPlayerID});
    socket.emit('spawn', player);
    socket.broadcast.emit('spawn', player);
    

    for(var playerID in players){
        if(playerID != thisPlayerID){
            socket.emit('spawn', players[playerID]);
        }
    }

    socket.on('updatePosition', function(data){
        player.position.x = data.position.x;
        player.position.y = data.position.y;
        player.position.z = data.position.z; 
        console.log(data);
        socket.broadcast.emit('updatePosition', player);
    });

    socket.on('updatePoint', function(data){
        player.point += data.point;
        if(player.point >= 20){
            socket.broadcast.emit('playerWin', player)
        }
    });

    socket.on('disconnect', function(){
        console.log('Player '+player.id+' disconnect');
        delete players[thisPlayerID];
        delete sockets[thisPlayerID];
        socket.broadcast.emit('disconnected', player);
    });
});