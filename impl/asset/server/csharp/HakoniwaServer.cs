using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HakoniwaGrpc;

namespace HakoniwaService
{
    class HakoniwaServer : HakoniwaCoreService.HakoniwaCoreServiceBase
    {
        private static Server server;
        static public void StartServer(string ipaddr, int portno)
        {
            if (HakoniwaServer.server != null)
            {
                throw new InvalidOperationException();
            }
            HakoniwaServer.server = new Server
            {
                Services = { HakoniwaCoreService.BindService(new HakoniwaServer()) },
                Ports = { new ServerPort(ipaddr, portno, ServerCredentials.Insecure) }
            };
            server.Start();
        }
        static public void ShutdownServer()
        {
            HakoniwaServer.server.ShutdownAsync().Wait();
            HakoniwaServer.server = null;
        }
        public override Task<HakoniwaReply> Register(HakoniwaAssetInfo request, ServerCallContext context)
        {
            Console.WriteLine("Register");
            return Task.FromResult(new HakoniwaReply
            {
                Ercd = "OK"
            });
        }
        public override Task<HakoniwaReply> Unregister(HakoniwaAssetInfo request, ServerCallContext context)
        {
            Console.WriteLine("Unregister");
            return Task.FromResult(new HakoniwaReply
            {
                Ercd = "OK"
            });
        }
    }


}
