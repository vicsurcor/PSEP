// client.js
 
const WebSocket = require('ws')
const url = 'ws://localhost:8080'
const connection = new WebSocket(url)
const readline = require('readline');
const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});
 
connection.onopen = () => {
  connection.send('Message From Client') 
}
 
connection.onerror = (error) => {
  console.log(`WebSocket error: ${error}`)
}
 
connection.onmessage = (e) => {
  console.log(e.data)
  rl.on('line', (answer) => {
    connection.send(answer)
  }
  );
  rl.close();
}
