﻿using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace clientesincrono {
    public class SynchronousSocketClient {

        public static void StartClient() {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPAddress ipAddress = getLocalIpAddress();//MAC OS
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test");

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                } catch (ArgumentNullException ane) {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                } catch (SocketException se) {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                } catch (Exception e) {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        static IPAddress getLocalIpAddress() {
            IPAddress ipAddress = null;
            try {
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces()) {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses) {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork) {
                                ipAddress = addrInfo.Address;
                            }
                        }
                    }
                }
                if (ipAddress == null) {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
                    ipAddress = ipHostInfo.AddressList[0];
                }
            } catch (Exception) { }
            return ipAddress;
        }

        public static int Main(String[] args) {
            //En MACOS porque no se puede ordenar el orden de arranque: servidor, cliente
            Thread.Sleep(4000);
            StartClient();
            return 0;
        }
    }
}