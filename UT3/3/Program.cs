using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ipaddress
{
    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo;
            IPAddress ipAddress;
            try
            {
                ipHostInfo = Dns.GetHostEntry("www.google.es");
                ipAddress = ipHostInfo.AddressList[0];
                Console.WriteLine("La dirección de google es: {0}", ipAddress.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine("No se puede resolver la dirección de google.");
            }
            //ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());//PUEDE DAR ERROR EN MAC
            //ipAddress = ipHostInfo.AddressList[0];
            ipAddress = getLocalIpAddres();
            Console.WriteLine("La dirección de esta máquina es: {0}", ipAddress);

        }
        static IPAddress getLocalIpAddres()
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
            }
            catch (Exception) { }
            if (ipAddress == null)
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
                ipAddress = ipHostInfo.AddressList[0];
            }
            return ipAddress;
        }
    }
}