using System;
using System.Collections.Generic;
using System.Text;

namespace HakoniwaService
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            string ipaddr = "172.25.144.1";
            int portno = 50051;

            Console.WriteLine("ipaddr=" + ipaddr + " portno=" + portno.ToString());
            Console.WriteLine("Hello World!!");
            HakoniwaServer.StartServer(ipaddr, portno);


            Console.WriteLine("Press any key to shutdown the server...");
            Console.ReadKey();

            HakoniwaServer.ShutdownServer();

        }
    }
}
