using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        private static SimulationController simulator = new SimulationController();
        //TODO

        static private void RemoveFirstEvent()
        {
        }
        static public SimulationController GetSimulator()
        {
            return simulator;
        }

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
            if (simulator.GetState() != HakoniwaSimulationState.Stopped) {
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Perm
                }); ;

            }
            if (simulator.asset_mgr.Register(request.Name))
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
            if (simulator.GetState() != HakoniwaSimulationState.Stopped)
            {
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Perm
                }); ;

            }
            simulator.asset_mgr.Unregister(request.Name);
            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }
        public override async Task AssetNotificationStart(AssetInfo request, IServerStreamWriter<AssetNotification> responseStream, ServerCallContext context)
        {
            //Asset 存在チェック
            if (!simulator.asset_mgr.IsExist(request.Name))
            {
                //未登録のアセットからの要求
                return;
            }
            while (true)
            {
                //アセットイベントチェック
                AssetEvent aev = simulator.asset_mgr.GetEvent(request.Name);
                if (aev == null)
                {
                    await Task.Delay(1000);
                    continue;
                }
                AssetNotificationEvent ev = aev.GetEvent();
                if (ev == AssetNotificationEvent.None)
                {
                    //終了
                    break;
                }
                //イベント通知
                AssetNotification req = new AssetNotification();
                req.Event = ev;
                Console.WriteLine("Send command:" + req.Event);
                await responseStream.WriteAsync(req);
            }
        }

        public override Task<NormalReply> AssetNotificationFeedback(AssetNotificationReply feedback, ServerCallContext context)
        {
            Console.WriteLine("AssetNotificationFeedback:" + feedback.Event + " Asset=" + feedback.Asset.Name + " ercd=" + feedback.Ercd);
            if (!simulator.asset_mgr.IsExist(feedback.Asset.Name))
            {
                //未登録のアセットからの要求
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Inval
                });
            }
            bool isOk = (feedback.Ercd == ErrorCode.Ok);
            switch (feedback.Event)
            {
                case AssetNotificationEvent.None:
                    break;
                case AssetNotificationEvent.Start:
                    simulator.StartFeedback(isOk);
                    break;
                case AssetNotificationEvent.End:
                    simulator.StopFeedback(isOk);
                    break;
                default:
                    //TODO
                    break;
            }
            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }
        public override Task<AssetInfoList> GetAssetList(Empty empty, ServerCallContext context)
        {
            List<AssetEntry> list = simulator.asset_mgr.GetAssetList();
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
