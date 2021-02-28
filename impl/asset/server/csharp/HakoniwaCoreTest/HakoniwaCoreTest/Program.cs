using System;
using Hakoniwa.Core;

namespace HakoniwaCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string ipaddr = "172.30.0.1";
            int portno = 50051;

            Console.WriteLine("ipaddr=" + ipaddr + " portno=" + portno.ToString());
            Console.WriteLine("Hello World!!");
            HakoniwaServer.StartServer(ipaddr, portno);

            Console.WriteLine("Press any key to Start...");
            Console.ReadKey();

            SimulationController simulator = HakoniwaServer.GetSimulator();
            simulator.Start();

            Console.WriteLine("Press any key to End...");
            Console.ReadKey();
            simulator.Stop();


            Console.WriteLine("Press any key to Terminate...");
            Console.ReadKey();
            simulator.Terminate();

            Console.WriteLine("Press any key to shutdown the server...");
            Console.ReadKey();

            HakoniwaServer.ShutdownServer();
        }
    }
}
