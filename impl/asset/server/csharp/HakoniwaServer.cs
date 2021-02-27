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
        private static AssetManager asset_mgr = new AssetManager();

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
            if (HakoniwaServer.asset_mgr.Register(request.Name))
            {
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Ok
                });
            }
            else
            {
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Exist
                }); ;
            }
        }
        public override Task<NormalReply> Unregister(AssetInfo request, ServerCallContext context)
        {
            Console.WriteLine("Unregister:" + request.Name);
            HakoniwaServer.asset_mgr.Unregister(request.Name);
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

            //Console.WriteLine("Press any key to next event...");
            //Console.ReadKey();


            req = new AssetNotification();
            req.Event = AssetNotificationEvent.End;
            Console.WriteLine("Send command:" + req.Event);
            await responseStream.WriteAsync(req);
            Console.WriteLine("END");
        }

        public override Task<NormalReply> AssetNotificationFeedback(AssetNotificationReply feedback, ServerCallContext context)
        {
            Console.WriteLine("AssetNotificationFeedback:" + feedback.Event + " Asset=" + feedback.Asset.Name + " ercd=" + feedback.Ercd);
            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }
        public override Task<AssetInfoList> GetAssetList(Empty empty, ServerCallContext context)
        {
            List<AssetEntry> list = asset_mgr.GetAssetList();
            AssetInfoList ret_list = new AssetInfoList();
            foreach (var entry in list)
            {
                AssetInfo info = new AssetInfo();
                info.Name = entry.GetName();
                ret_list.Assets.Add(info);
            }
            return Task.FromResult(ret_list);
        }
    }


}
