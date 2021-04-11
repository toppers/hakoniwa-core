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

            var controller = new WorldController(filepath);

            controller.Execute();

            Console.WriteLine("Shutdown START");
            RpcServer.ShutdownServer();
            Console.WriteLine("Shutdown END");
        }
    }
}
