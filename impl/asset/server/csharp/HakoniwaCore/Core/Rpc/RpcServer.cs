﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Simulation;
using HakoniwaGrpc;

namespace Hakoniwa.Core.Rpc
{
    public class RpcServer : CoreService.CoreServiceBase
    {
        private static Server server;

        static public SimulationController GetSimulator()
        {
            return SimulationController.Get();
        }

        static public void StartServer(string ipaddr, int portno)
        {
            if (RpcServer.server != null)
            {
                throw new InvalidOperationException();
            }
            RpcServer.server = new Server
            {
                Services = { CoreService.BindService(new RpcServer()) },
                Ports = { new ServerPort(ipaddr, portno, ServerCredentials.Insecure) }
            };
            server.Start();
        }
        static public void ShutdownServer()
        {
            RpcServer.server.ShutdownAsync().Wait();
            RpcServer.server = null;
        }

        public override Task<NormalReply> Register(AssetInfo request, ServerCallContext context)
        {
            if (RpcServer.GetSimulator().GetState() != SimulationState.Stopped) {
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Perm
                }); ;

            }
            if (RpcServer.GetSimulator().asset_mgr.RegisterOutsideAsset(request.Name))
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
            if (RpcServer.GetSimulator().GetState() != SimulationState.Stopped)
            {
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Perm
                }); ;

            }
            RpcServer.GetSimulator().asset_mgr.Unregister(request.Name);
            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }
        public override Task<NormalReply> StartSimulation(Empty empty, ServerCallContext context)
        {
            if (RpcServer.GetSimulator().Start())
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
                    Ercd = ErrorCode.Inval
                });
            }
        }
        public override Task<NormalReply> StopSimulation(Empty empty, ServerCallContext context)
        {
            if (RpcServer.GetSimulator().Stop())
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
                    Ercd = ErrorCode.Inval
                });
            }
        }
        public override Task<NormalReply> ResetSimulation(Empty empty, ServerCallContext context)
        {
            if (RpcServer.GetSimulator().ResetRequest())
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
                    Ercd = ErrorCode.Inval
                });
            }
        }

        private AssetNotificationEvent InternalEvent2RpcEvent(CoreAssetNotificationEvent iev)
        {
            switch (iev)
            {
                case CoreAssetNotificationEvent.Start:
                    return AssetNotificationEvent.Start;
                case CoreAssetNotificationEvent.Stop:
                    return AssetNotificationEvent.End;
                default:
                    return AssetNotificationEvent.None;
            }
        }
        public override async Task AssetNotificationStart(AssetInfo request, IServerStreamWriter<AssetNotification> responseStream, ServerCallContext context)
        {
            //Asset 存在チェック
            if (!RpcServer.GetSimulator().asset_mgr.IsExist(request.Name))
            {
                //未登録のアセットからの要求
                return;
            }
            while (true)
            {
                //アセットイベントチェック
                AssetEvent aev = RpcServer.GetSimulator().asset_mgr.GetEvent(request.Name);
                if (aev == null)
                {
                    await Task.Delay(1000);
                    continue;
                }
                CoreAssetNotificationEvent ev = aev.GetEvent();
                if (ev == CoreAssetNotificationEvent.None)
                {
                    //終了
                    break;
                }
                //イベント通知
                AssetNotification req = new AssetNotification();
                req.Event = InternalEvent2RpcEvent(ev);
                Console.WriteLine("Send command:" + req.Event);
                await responseStream.WriteAsync(req);
            }
        }

        public override Task<NormalReply> AssetNotificationFeedback(AssetNotificationReply feedback, ServerCallContext context)
        {
            Console.WriteLine("AssetNotificationFeedback:" + feedback.Event + " Asset=" + feedback.Asset.Name + " ercd=" + feedback.Ercd);
            if (!RpcServer.GetSimulator().asset_mgr.IsExist(feedback.Asset.Name))
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
                    RpcServer.GetSimulator().StartFeedback(isOk);
                    break;
                case AssetNotificationEvent.End:
                    RpcServer.GetSimulator().StopFeedback(isOk);
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
            List<AssetEntry> list = RpcServer.GetSimulator().asset_mgr.GetAssetList();
            AssetInfoList ret_list = new AssetInfoList();
            foreach (var entry in list)
            {
                AssetInfo info = new AssetInfo();
                info.Name = entry.GetName();
                ret_list.Assets.Add(info);
            }
            return Task.FromResult(ret_list);
        }
        public override Task<NormalReply> FlushSimulationTimeSyncInfo(SimulationTimeSyncOutputFile request, ServerCallContext context)
        {

            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }

    }
}
