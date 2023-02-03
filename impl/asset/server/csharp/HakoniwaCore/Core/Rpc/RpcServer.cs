#if NO_USE_GRPC
#else
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Simulation;
using Hakoniwa.Core.Utils;
using Hakoniwa.Core.Utils.Logger;
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
            SimpleLogger.Get().Log(Level.INFO, "Register:" + request.Name);
            if (RpcServer.GetSimulator().GetState() != SimulationState.Stopped) {
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Perm
                }); ;

            }
            if (RpcServer.GetSimulator().RegisterOutsideAsset(request.Name))
            {
                SimpleLogger.Get().Log(Level.INFO, "Register Success: " + request.Name);
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Ok
                });
            }
            else
            {
                SimpleLogger.Get().Log(Level.INFO, "Register Failed: " + request.Name);
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Exist
                }); ;
            }
        }
        public override Task<NormalReply> Unregister(AssetInfo request, ServerCallContext context)
        {
            SimpleLogger.Get().Log(Level.INFO, "Unregister:" + request.Name);
            if (RpcServer.GetSimulator().GetState() != SimulationState.Stopped)
            {
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Perm
                }); ;

            }
            RpcServer.GetSimulator().Unregister(request.Name);
            return Task.FromResult(new NormalReply
            {
                Ercd = ErrorCode.Ok
            });
        }
        public override Task<NormalReply> StartSimulation(Empty empty, ServerCallContext context)
        {
            SimpleLogger.Get().Log(Level.INFO, "StartSimulation");
            try
            {
                if (RpcServer.GetSimulator().Start())
                {
                    SimpleLogger.Get().Log(Level.INFO, "StartSimulation:Success");
                    return Task.FromResult(new NormalReply
                    {
                        Ercd = ErrorCode.Ok
                    });
                }
                else
                {
                    SimpleLogger.Get().Log(Level.INFO, "StartSimulation:Failed");
                    return Task.FromResult(new NormalReply
                    {
                        Ercd = ErrorCode.Inval
                    });
                }
            }
            catch (Exception e)
            {
                SimpleLogger.Get().Log(Level.ERROR, e);
                SimpleLogger.Get().Log(Level.INFO, "StartSimulation:Exception");
                return Task.FromResult(new NormalReply
                {
                    Ercd = ErrorCode.Inval
                });
            }
        }
        public override Task<NormalReply> StopSimulation(Empty empty, ServerCallContext context)
        {
            SimpleLogger.Get().Log(Level.INFO, "StopSimulation");
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
            SimpleLogger.Get().Log(Level.INFO, "ResetSimulation");
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
        public override Task<SimStatReply> GetSimStatus(Empty empty, ServerCallContext context)
        {
            var reply = new SimStatReply();
            reply.Ercd = ErrorCode.Ok;
            switch (RpcServer.GetSimulator().GetState())
            {
                case SimulationState.Stopped:
                    reply.Status = SimulationStatus.StatusStopped;
                    break;
                case SimulationState.Runnable:
                    reply.Status = SimulationStatus.StatusRunnable;
                    break;
                case SimulationState.Running:
                    reply.Status = SimulationStatus.StatusRunning;
                    break;
                case SimulationState.Stopping:
                    reply.Status = SimulationStatus.StatusStopping;
                    break;
                case SimulationState.Terminated:
                default:
                    reply.Status = SimulationStatus.StatusTerminated;
                    break;
            }
            return Task.FromResult(reply);
        }

        private AssetNotificationEvent InternalEvent2RpcEvent(CoreAssetNotificationEvent iev)
        {
            switch (iev)
            {
                case CoreAssetNotificationEvent.Start:
                    return AssetNotificationEvent.Start;
                case CoreAssetNotificationEvent.Stop:
                    return AssetNotificationEvent.Stop;
                default:
                    return AssetNotificationEvent.None;
            }
        }
        public override async Task AssetNotificationStart(AssetInfo request, IServerStreamWriter<AssetNotification> responseStream, ServerCallContext context)
        {
            //Asset 存在チェック
            if (!RpcServer.GetSimulator().IsExist(request.Name))
            {
                //未登録のアセットからの要求
                SimpleLogger.Get().Log(Level.ERROR, "AssetNotificationStart: unkown asset request:" + request.Name);
                return;
            }
            SimpleLogger.Get().Log(Level.INFO, "AssetNotificationStart:" + request.Name);
            while (true)
            {
                AssetNotification req = null;
                //アセットイベントチェック
                AssetEvent aev = RpcServer.GetSimulator().GetEvent(request.Name);
                if (aev == null)
                {
                    await Task.Delay(1000);
                    //ハートビート監視する
                    req = new AssetNotification();
                    req.Event = AssetNotificationEvent.Heartbeat;
                    await responseStream.WriteAsync(req);
                    continue;
                }
                CoreAssetNotificationEvent ev = aev.GetEvent();
                if (ev == CoreAssetNotificationEvent.None)
                {
                    //終了
                    break;
                }
                //イベント通知
                req = new AssetNotification();
                req.Event = InternalEvent2RpcEvent(ev);
                await responseStream.WriteAsync(req);
            }
        }

        public override Task<NormalReply> AssetNotificationFeedback(AssetNotificationReply feedback, ServerCallContext context)
        {
            //SimpleLogger.Get().Log(Level.INFO, "AssetNotificationFeedback:" + feedback.Event + " Asset=" + feedback.Asset.Name + " ercd=" + feedback.Ercd);
            if (!RpcServer.GetSimulator().IsExist(feedback.Asset.Name))
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
                    RpcServer.GetSimulator().StartFeedback(feedback.Asset.Name, isOk);
                    break;
                case AssetNotificationEvent.Stop:
                    RpcServer.GetSimulator().StopFeedback(feedback.Asset.Name, isOk);
                    break;
                case AssetNotificationEvent.Heartbeat:
                    RpcServer.GetSimulator().UpdateTime(feedback.Asset.Name);
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
            List<AssetEntry> list = RpcServer.GetSimulator().GetAssetList();
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
#endif