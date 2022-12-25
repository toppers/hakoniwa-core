using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hakoniwa.Core.Simulation;
using Hakoniwa.Core.Utils.Logger;
using HakoniwaGrpc;
using static HakoniwaGrpc.CoreService;

namespace Hakoniwa.Core.Rpc
{
    public class RpcClient
    {
        private static CoreServiceClient client;

        static public void StartClient(string ipaddr, int portno)
        {
            var channel = new Channel(ipaddr, portno, ChannelCredentials.Insecure);
            RpcClient.client = new CoreServiceClient(channel);

            if (RpcClient.client == null)
            {
                throw new InvalidOperationException();
            }
        }

        static public CoreServiceClient Get()
        {
            return client;
        }

        static public bool Register(string asset_name)
        {
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
        static public SimulationState GetSimStatus()
        {
            var empty = new Empty();
            var res = client.GetSimStatus(empty);
            if (res.Ercd != ErrorCode.Ok)
            {
                return SimulationState.Terminated;
            }
            switch (res.Status)
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

        //AssetNotificationStartStream
        static private bool AssetNotificationFeedback(string asset_name, AssetNotificationEvent ev, bool result)
        {
            AssetNotificationReply arg = new AssetNotificationReply();
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
            return AssetNotificationFeedback(asset_name, AssetNotificationEvent.End, result);
        }
        static public bool AssetNotificationFeedbackHeartbeat(string asset_name, bool result)
        {
            return AssetNotificationFeedback(asset_name, AssetNotificationEvent.Heartbeat, result);
        }
        static public long NotifySimtime(string asset_name, long asset_time)
        {
            var req = new NotifySimtimeRequest();
            req.Asset.Name = asset_name;
            req.AssetTime = asset_time;
            var res = client.NotifySimtime(req);
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "NotifySimtime(" + asset_time + ") Success");
                return res.MasterTime;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "NotifySimtime(" + asset_time + ") Failed: ercd=" + res.Ercd);
                return 0;
            }
        }
        static public int CreatePduChannel(string asset_name, int channel_id, int pdu_size)
        {
            var req = new CreatePduChannelRequest();
            req.AssetName = asset_name;
            req.ChannelId = channel_id;
            req.PduSize = pdu_size;
            var res = client.CreatePduChannel(req);
            if (res.Ercd == ErrorCode.Ok)
            {
                SimpleLogger.Get().Log(Level.INFO, "CreatePduChannel(" + asset_name + ") Success");
                return res.MasterUdpPort;
            }
            else
            {
                SimpleLogger.Get().Log(Level.ERROR, "CreatePduChannel(" + asset_name + ") Failed: ercd=" + res.Ercd);
                return -1;
            }
        }
        static public bool SubscribePduChannel(string asset_name, int channel_id, int pdu_size, string ipaddr, int port)
        {
            var req = new SubscribePduChannelRequest();
            req.AssetName = asset_name;
            req.ChannelId = channel_id;
            req.PduSize = pdu_size;
            req.ListenUdpIpPort = ipaddr + ":" + port;
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
