using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SyncServer
{
    public class SynchronousSocketListener
    {

        public static int TAM = 1024;

        // Incoming data from the client.  
        public static string data = null;
        public static Dictionary<int, int> list = new Dictionary<int, int>();

        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[TAM];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = getLocalIpAddress();//MAC OS
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;
                    int bytesRec = 0;
                    byte[] msg = null;
                    while (true){
                        bytesRec = handler.Receive(bytes);
                        data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        // An incoming connection needs to be processed.  
                        while (bytesRec == TAM)
                        {
                            bytesRec = handler.Receive(bytes);
                            data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        }

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
                        handler.Send(msg);
                    }
                    // Sends disconnect confirmation
                    msg = Encoding.ASCII.GetBytes("Solicitud de desconexion");
                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        static IPAddress getLocalIpAddress()
        {
            IPAddress ipAddress = null;
            try
            {
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipAddress = addrInfo.Address;
                            }
                        }
                    }
                }
                if (ipAddress == null)
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
                    ipAddress = ipHostInfo.AddressList[0];
                }
            }
            catch (Exception) { }
            return ipAddress;
        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }
    }
}