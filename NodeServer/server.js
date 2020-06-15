const express = require('express');
const path = require('path');
const { createServer } = require('http');
const WebSocket = require('ws');

const app = express();

const server = createServer(app);
const wss = new WebSocket.Server({ server });

var clients = [];

wss.on('connection', function(ws) {
  clients.push(ws);

  ws.on('message', function(data) {
    console.log("message:", data);
    for (var i = 0; i < clients.length; i++) {
      clients[i].send(data);
    }
  });

  ws.on('close', function() {

  });
});

server.listen(8000, function() {
  console.log('Listening on http://localhost:8000');
});
