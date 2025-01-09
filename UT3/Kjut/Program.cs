//1. Modify the code of the synchronous server so that it displays the IP and port of the client that has connected and precedes the message sent by the synchronous client. 
//For example:
//192.168.1.103:60353 sent: This is a test<EOF>

// Show the data on the console.  
Console.WriteLine("Text received from {0} : {1}", handler.RemoteEndPoint, data);

//2. Modify the code of the synchronous client so that in a loop it requests a value by console that is a "0", a "1" or a "2". 
//If it is none of these values the program should terminate, otherwise this value should be sent to the synchronous server.

Console.WriteLine("Enter integer [0,1,2]");
// Encode the data string into a byte array.  
msg = Encoding.ASCII.GetBytes(Console.ReadLine());
Console.WriteLine(msg);
                        
// Send the data through the socket.  
bytesSent = sender.Send(msg);

// Receive the response from the remote device.  
bytesRec = sender.Receive(bytes);

data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
int.TryParse(data, out int num);
if (num < 0 || num > 2) {
    Console.WriteLine("Desconectando");
    sender.Shutdown(SocketShutdown.Both);
    sender.Close();
    break;
}

//3. Modify the synchronous server code to display statistics with the number of zeros, ones and twos received each time a value is received.

                        int.TryParse(data, out int num);
                        if (num == 0) {
                            if (!list.ContainsKey(num)) {
                                list.Add(num, 0);
                            }
                            list[num]++;
                        }
                        else if (num == 1) {
                            if (!list.ContainsKey(num)) {
                                list.Add(num, 0);
                            }
                            list[num]++;
                        }
                        else if (num == 2) {
                            if (!list.ContainsKey(num)) {
                                list.Add(num, 0);
                            }
                            list[num]++;
                        }

                        Console.WriteLine("List data:  [0]: {0} [1]: {1} [2]: {2}", list.ElementAtOrDefault(0), list.ElementAtOrDefault(1), list.ElementAtOrDefault(2));
                        