var shortID = require('shortid');
var Vector3 = require('./Vector3.js');

module.exports = class Player{
    constructor(){
        this.name = 'coin'
        this.id = shortID.generate();
        this.position = new Vector3(); 
    }
}