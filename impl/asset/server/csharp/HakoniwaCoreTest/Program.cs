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
            string filepath = @"..\..\..\core_config.json";
            AssetConfigLoader.Load(filepath);
            string ipaddr = AssetConfigLoader.core_config.core_ipaddr;
            int portno = AssetConfigLoader.core_config.core_portno;

            Console.WriteLine("ipaddr=" + ipaddr + " portno=" + portno.ToString());
            RpcServer.StartServer(ipaddr, portno);

            Console.WriteLine("Press any key to shutdown the server...");
            Console.ReadKey();

            Console.WriteLine("Shutdown START");
            RpcServer.ShutdownServer();
            Console.WriteLine("Shutdown END");
        }
    }
}
