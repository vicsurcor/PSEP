//1. Modify the code of the synchronous server so that it displays the IP and port of the client that has connected and precedes the message sent by the synchronous client. 
//For example:
//192.168.1.103:60353 sent: This is a test<EOF>

// Show the data on the console.  
Console.WriteLine("Text received from {0} : {1}", handler.RemoteEndPoint, data);

//2. Modify the code of the synchronous client so that in a loop it requests a value by console that is a "0", a "1" or a "2". 
//If it is none of these values the program should terminate, otherwise this value should be sent to the synchronous server.