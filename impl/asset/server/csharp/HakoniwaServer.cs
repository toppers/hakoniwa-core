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
    class HakoniwaServer : CoreService.CoreServiceBase
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
                Services = { CoreService.BindService(new HakoniwaServer()) },
                Ports = { new ServerPort(ipaddr, portno, ServerCredentials.Insecure) }
            };
            server.Start();
        }
        static public void ShutdownServer()
        {
            HakoniwaServer.server.ShutdownAsync().Wait();
            HakoniwaServer.server = null;
        }

        public override Task<NormalReply> Register(AssetInfo request, ServerCallContext context)
        {
            Console.WriteLine("Register:" + request.Name);
            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }
        public override Task<NormalReply> Unregister(AssetInfo request, ServerCallContext context)
        {
            Console.WriteLine("Unregister:" + request.Name);
            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }
        public override async Task AssetNotificationStart(AssetInfo request, IServerStreamWriter<AssetNotification> responseStream, ServerCallContext context)
        {
            AssetNotification req = new AssetNotification();
            req.Event = AssetNotificationEvent.Start;
            Console.WriteLine("Send command:" + req.Event);
            await responseStream.WriteAsync(req);


            req = new AssetNotification();
            req.Event = AssetNotificationEvent.End;
            Console.WriteLine("Send command:" + req.Event);
            await responseStream.WriteAsync(req);
            Console.WriteLine("END");
        }

        public override Task<NormalReply> AssetNotificationFeedback(AssetNotificationReply feedback, ServerCallContext context)
        {
            Console.WriteLine("CommandReply:" + feedback.Event + " Asset=" + feedback.Asset.Name + " ercd=" + feedback.Ercd);
            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }
    }


}
