using System;
using Hakoniwa.Core;
using Hakoniwa.Core.Rpc;
using Hakoniwa.Core.Simulation;
using Hakoniwa.PluggableAsset;

namespace HakoniwaCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filepath = @"C:\project\oss\hakoniwa-core\impl\asset\server\csharp\HakoniwaCoreTest\HakoniwaCoreTest\core_config.json";
            AssetConfigLoader.Load(filepath);
#if false
            string ipaddr = "172.18.144.1";
            int portno = 50051;

            Console.WriteLine("ipaddr=" + ipaddr + " portno=" + portno.ToString());
            RpcServer.StartServer(ipaddr, portno);

            LineBinaryStorageTest test = new LineBinaryStorageTest();
            test.DoTest();


            Console.WriteLine("Press any key to Start...");
            Console.ReadKey();

            SimulationController simulator = RpcServer.GetSimulator();
            simulator.Start();

            Console.WriteLine("Press any key to End...");
            Console.ReadKey();
            simulator.Stop();


            Console.WriteLine("Press any key to Terminate...");
            Console.ReadKey();
            simulator.Terminate();

            Console.WriteLine("Press any key to shutdown the server...");
            Console.ReadKey();

            RpcServer.ShutdownServer();
#endif
        }
    }
}
