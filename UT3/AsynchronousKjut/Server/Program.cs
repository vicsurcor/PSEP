using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml.Serialization;
using MsgJsonLib;

namespace AsyncSrv
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.   !!!!
        public const int BufferSize = 10; // 1024;
        
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
        
    }

    public class AsynchronousSocketListener
    {

        public static string data = null;
        public static SortedDictionary<int, int> list;
        public static int TAM = 1024;
        
        private static int PORT = 11000;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }

        public static void StartListening()
        {

            
            // Establish the local endpoint for the socket.  
            IPAddress ipAddress = GetLocalIpAddress();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection at {0}...", localEndPoint);
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {

            byte[] bytes = new Byte[TAM];
            Console.Write("_"); // Trace
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            // Gets the amount of data that has been received from the network and 
            // is available to be read.
            if (handler.Available > 0)
            {
                Console.Write("0"); // Trace
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else
            {
                if (bytesRead > 0)
                {
                    Console.Write("1"); // Trace
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                }
                if (state.sb.Length >= 1)
                {
                    Console.Write("2");
                    Console.WriteLine(state.sb.ToString());
                    while (true)
                    {
                    
                        // Program is suspended while waiting for an incoming connection.  
                        list = new SortedDictionary<int, int>();
                        data = null;
                        byte[] msg = null;
                        while (true){

                            data = state.sb.ToString();
                
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

                            Console.WriteLine("List data: ([value, quantity])  {0}  {1}  {2}", list.ElementAtOrDefault(0), list.ElementAtOrDefault(1), list.ElementAtOrDefault(2));
                            
                            // Show the data on the console.  
                            // Console.WriteLine("Text received: {0}", data)
                            Console.WriteLine("Text received from {0} : {1}", handler.RemoteEndPoint, data);
                            // If the data is a disconnect request
                            // if (data == "Exit") {
                            //     break;
                            // }
                            
                            if (num < 0 || num > 2) {
                                break;
                            }

                            // Echo the data back to the client.  
                            msg = Encoding.ASCII.GetBytes(data);
                            Send(handler, msg);
                        }
                        // Sends disconnect confirmation
                        msg = Encoding.ASCII.GetBytes("Solicitud de desconexion");
                        Send(handler, msg);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();

                    }
                }
                else
                {
                    // If nothing has been received
                    Console.Write("3"); // Trace
                }
            }

        }

        private static void Send(Socket handler, byte[] data)
        {
            // // Convert the message
            // XmlSerializer serializer = new XmlSerializer(typeof(Message));
            // Stream stream = new MemoryStream();
            // serializer.Serialize(stream, data);
            // byte[] byteData = ((MemoryStream)stream).ToArray();
            // // Begin sending the data to the remote device.  
            // handler.BeginSend(byteData, 0, byteData.Length, 0,
            // new AsyncCallback(SendCallback), handler);
            handler.BeginSend(data, 0, data.Length, 0,
                                 new AsyncCallback(SendCallback), handler);

        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
                Console.WriteLine(handler.Connected);
                // handler.Shutdown(SocketShutdown.Both);
                // Console.WriteLine(handler.Connected);
                // handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static IPAddress GetLocalIpAddress()
        {
            List<IPAddress> ipAddressList = new List<IPAddress>();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            int t = ipHostInfo.AddressList.Length;
            string ip;
            for (int i = 0; i < t; i++)
            {
                ip = ipHostInfo.AddressList[i].ToString();
                if (ip.Contains(".") && !ip.Equals("127.0.0.1"))
                {
                    ipAddressList.Add(ipHostInfo.AddressList[i]);
                }
            }
            if (ipAddressList.Count == 1)
            {
                return ipAddressList[0];
            }
            else
            {
                int i = 0;
                foreach (IPAddress ipa in ipAddressList)
                {
                    Console.WriteLine($"[{i++}]: {ipa}");
                }
                t = ipAddressList.Count - 1;
                System.Console.Write($"Opción [0-{t}]: ");
                string s = Console.ReadLine();
                if (Int32.TryParse(s, out int j))
                {
                    if ((j >= 0) && (j <= t))
                    {
                        return ipAddressList[j];
                    }
                }
                return null;
            }
        }

    }
}