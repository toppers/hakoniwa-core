#if NO_USE_GRPC
#else
using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hakoniwa.Core.Simulation;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset;
using HakoniwaGrpc;
using static HakoniwaGrpc.CoreService;

namespace Hakoniwa.Core.Rpc
{
    public struct SimulationAttrs
    {
        public long master_time;
        public SimulationState state;
        public bool is_pdu_created;
        public bool is_simulation_mode;
        public bool is_pdu_sync_mode;
    }
    public interface RpcClientCallback
    {
        void StartCallback();
        void StopCallback();
        void ResetCallback();
    }
    public class RpcClient
    {
        private static CoreServiceClient client;
        private static RpcClientCallback callback;

        static public void StartClient(string ipaddr, int portno)
        {
            SimpleLogger.Get().Log(Level.INFO, "StartClient:ipaddr=" + ipaddr);
            SimpleLogger.Get().Log(Level.INFO, "StartClient:portno=" + portno);
            var channel = new Channel(ipaddr, portno, ChannelCredentials.Insecure);
            RpcClient.client = new CoreServiceClient(channel);

            if (RpcClient.client == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                SimpleLogger.Get().Log(Level.INFO, "Client is OK");
            }
        }

        static public CoreServiceClient Get()
        {
            return client;
        }

        static public bool Register(string asset_name, RpcClientCallback obj)
        {
            callback = obj;
            AssetInfo request = new AssetInfo();
            request.Name = asset_name;
            var res = client.Register(request);
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "Register Success:" + request.Name);
                return true;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "Register Failed:" + request.Name + " ercd=" + res.Ercd);
                return false;
            }
        }
        static public bool Unregister(string asset_name)
        {
            AssetInfo request = new AssetInfo();
            request.Name = asset_name;
            var res = client.Unregister(request);
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "Unregister Success:" + request.Name);
                return true;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "Unregister Failed:" + request.Name + " ercd=" + res.Ercd);
                return false;
            }
        }

        static SimulationState GetState(SimulationStatus status)
        {
            //SimpleLogger.Get().Log(Level.INFO, "GetState(): status=" + status);
            switch (status)
            {
                case SimulationStatus.StatusStopped:
                    return SimulationState.Stopped;
                case SimulationStatus.StatusStopping:
                    return SimulationState.Stopping;
                case SimulationStatus.StatusRunnable:
                    return SimulationState.Runnable;
                case SimulationStatus.StatusRunning:
                    return SimulationState.Running;
                default:
                    return SimulationState.Terminated;
            }
        }

        static public SimulationState GetSimStatus()
        {
            var empty = new Empty();
            var res = client.GetSimStatus(empty);
            if (res.Ercd != ErrorCode.Ok)
            {
                return SimulationState.Terminated;
            }
            return GetState(res.Status);
        }
        static public bool StartSimulation()
        {
            var res = client.StartSimulation(new Empty());
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "StartSimulation Success");
                return true;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "StartSimulation Failed: ercd=" + res.Ercd);
                return false;
            }
        }
        static public bool StoptSimulation()
        {
            var res = client.StopSimulation(new Empty());
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "StoptSimulation Success");
                return true;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "StoptSimulation Failed: ercd=" + res.Ercd);
                return false;
            }
        }
        static public bool ResettSimulation()
        {
            var res = client.ResetSimulation(new Empty());
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "ResetSimulation Success");
                return true;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "ResetSimulation Failed: ercd=" + res.Ercd);
                return false;
            }
        }

        static public async Task AssetNotificationStartAsync(string asset_name)
        {
            var asset_info = new AssetInfo();
            asset_info.Name = asset_name;
            var call = client.AssetNotificationStart(asset_info);

            while (true)
            {
                await call.ResponseStream.MoveNext();
                var ev = call.ResponseStream.Current.Event;
                switch (ev) {
                    case AssetNotificationEvent.None:
                        break;
                    case AssetNotificationEvent.Start:
                        SimpleLogger.Get().Log(Level.INFO, "Recv Event: " + ev);
                        callback.StartCallback();
                        break;
                    case AssetNotificationEvent.Stop:
                        SimpleLogger.Get().Log(Level.INFO, "Recv Event: " + ev);
                        callback.StopCallback();
                        break;
                    case AssetNotificationEvent.Reset:
                        SimpleLogger.Get().Log(Level.INFO, "Recv Event: " + ev);
                        callback.ResetCallback();
                        break;
                    case AssetNotificationEvent.Heartbeat:
                        break;
                    default:
                        SimpleLogger.Get().Log(Level.ERROR, "Invalid Event" + ev);
                        break;
                }
            }
        }


        static private bool AssetNotificationFeedback(string asset_name, AssetNotificationEvent ev, bool result)
        {
            AssetNotificationReply arg = new AssetNotificationReply();
            arg.Asset = new AssetInfo();
            arg.Asset.Name = asset_name;
            arg.Event = ev;
            if (result)
            {
                arg.Ercd = ErrorCode.Ok;
            }
            else
            {
                arg.Ercd = ErrorCode.Inval;
            }
            var res = client.AssetNotificationFeedback(arg);
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "AssetNotificationFeedback(" + ev + ") Success");
                return true;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "AssetNotificationFeedback(" + ev + ") Failed: ercd=" + res.Ercd);
                return false;
            }
        }
        static public bool AssetNotificationFeedbackStart(string asset_name, bool result)
        {
            return AssetNotificationFeedback(asset_name, AssetNotificationEvent.Start, result);
        }
        static public bool AssetNotificationFeedbackStop(string asset_name, bool result)
        {
            return AssetNotificationFeedback(asset_name, AssetNotificationEvent.Stop, result);
        }
        static public bool AssetNotificationFeedbackReset(string asset_name, bool result)
        {
            return AssetNotificationFeedback(asset_name, AssetNotificationEvent.Reset, result);
        }
        static public bool AssetNotificationFeedbackHeartbeat(string asset_name, bool result)
        {
            return AssetNotificationFeedback(asset_name, AssetNotificationEvent.Heartbeat, result);
        }
        static public bool NotifySimtime(string asset_name, long asset_time, bool is_read_pdu_done, bool is_write_pdu_done, ref SimulationAttrs attrs)
        {
            var req = new NotifySimtimeRequest();
            req.Asset = new AssetInfo();
            req.Asset.Name = asset_name;
            req.AssetTime = asset_time;
            req.IsReadPduDone = is_read_pdu_done;
            req.IsWritePduDone = is_write_pdu_done;
            var res = client.NotifySimtime(req);
            if (res.Ercd == ErrorCode.Ok)
            {
                //SimpleLogger.Get().Log(Level.INFO, "NotifySimtime(" + asset_time + ") Success");
                attrs.master_time = res.MasterTime;
                attrs.is_pdu_created = res.IsPduCreated;
                attrs.is_pdu_sync_mode = res.IsPduSyncMode;
                attrs.is_simulation_mode = res.IsSimulationMode;
                attrs.state = GetState(res.Status);
                return true;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "NotifySimtime(" + asset_time + ") Failed: ercd=" + res.Ercd);
                return false;
            }
        }
        static public int CreatePduChannel(string asset_name, string robo_name, int channel_id, int pdu_size, string method_type)
        {
            var req = new CreatePduChannelRequest();
            req.AssetName = asset_name;
            req.RoboName = robo_name;
            req.ChannelId = channel_id;
            req.PduSize = pdu_size;
            req.MethodType = method_type;
            var res = client.CreatePduChannel(req);
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "CreatePduChannel(" + asset_name + ") Success");
                return res.Port;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "CreatePduChannel(" + asset_name + ") Failed: ercd=" + res.Ercd);
                return -1;
            }
        }
        static public bool SubscribePduChannel(string asset_name, string robo_name, int channel_id, int pdu_size, string ipaddr, int port, string method_type)
        {
            var req = new SubscribePduChannelRequest();
            req.AssetName = asset_name;
            req.RoboName = robo_name;
            req.ChannelId = channel_id;
            req.PduSize = pdu_size;
            req.ListenUdpIpPort = ipaddr + ":" + port;
            req.MethodType = method_type;
            var res = client.SubscribePduChannel(req);
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "SubscribePduChannel(" + asset_name + ") Success");
                return true;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "SubscribePduChannel(" + asset_name + ") Failed: ercd=" + res.Ercd);
                return false;
            }
        }
        /* end */
    }
}
#endif