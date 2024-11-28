using System;

using System.Net;

namespace ComunicacionPrimerosPaso

{

    class Program

    {

        static void Main(string[] args)

        {

            IPHostEntry ipHostInfo = Dns.GetHostEntry("www.google.es");

            IPAddress ipAddress = ipHostInfo.AddressList[0];

            Console.WriteLine("google address is: {0}",ipAddress.ToString());

            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            ipAddress = ipHostInfo.AddressList[0];

            Console.WriteLine("The address of this machine is : {0}", ipAddress);

}

}

}