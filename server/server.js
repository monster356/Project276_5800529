var io = require('socket.io')(process.env.PORT || 3000);

console.log("serverStarted");

var playarcount = 0;

//console.log(getRandomArbitrary(-5.00, 5.00));

io.on("connection", function (socket) {

    console.log("clientConnected");

    socket.broadcast.emit('spawn');
    playarcount++;

    for (i = 0; i < playarcount; i++) {
        socket.emit("spawn");
        console.log("spawn existing player");
    }

    socket.on("move", function (data) {

        console.log("moving",JSON.stringify(data));
        
        //socket.broadcast.emit("move",data)
    })

    socket.on("disconnect", function () {
        console.log("client disconnect");
        playarcount--;
    })
});