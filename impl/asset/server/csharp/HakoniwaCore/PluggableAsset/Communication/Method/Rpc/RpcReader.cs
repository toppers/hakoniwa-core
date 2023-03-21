#if NO_USE_GRPC
#else

using Hakoniwa.Core;
using Hakoniwa.Core.Rpc;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Communication.Method.Mqtt;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Hakoniwa.PluggableAsset.Communication.Method.Rpc
{
    class RpcReader: IIoReader
    {
        private static Dictionary<string, RpcReader> map = new Dictionary<string, RpcReader>();
        private static Thread thread;
        private static bool isUdpActive = false;
        public static void StartUdpServer()
        {
            if (isUdpActive)
            {
                return;
            }
            isUdpActive = true;
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }
        private static void ThreadMethod()
        {
            string localIpString = AssetConfigLoader.core_config.asset_ipaddr;
            IPAddress localAddress = IPAddress.Parse(localIpString);
            int localPort = AssetConfigLoader.core_config.pdu_udp_portno_asset;

            IPEndPoint localEP = new IPEndPoint(localAddress, localPort);
            UdpClient client = new System.Net.Sockets.UdpClient(localEP);
            SimpleLogger.Get().Log(Level.DEBUG, "UDP listen IP=" + localIpString + " port=" + localPort);

            //client = new UdpClient(AssetConfigLoader.core_config.pdu_udp_portno_asset);
            while (true)
            {
                int header_off = 12;
                IPEndPoint remoteEP = null;
                byte[] data = client.Receive(ref remoteEP);
                int channel_id = BitConverter.ToInt32(data, 0);
                int pdu_size = BitConverter.ToInt32(data, 4);
                int name_len = BitConverter.ToInt32(data, 8);
                byte[] robo_name_bytes = new byte[name_len];
                Buffer.BlockCopy(data, header_off, robo_name_bytes, 0, name_len);
                header_off += name_len;
                string robo_name = System.Text.Encoding.ASCII.GetString(robo_name_bytes);
                //SimpleLogger.Get().Log(Level.DEBUG, "recv channel_id=" + channel_id + " len=" + pdu_size);
                var obj = map[robo_name + ":" + channel_id];
                lock (obj.lockObj)
                {
                    if (obj.buffer == null)
                    {
                        obj.buffer = new byte[pdu_size];
                    }
                    Buffer.BlockCopy(data, header_off, obj.buffer, 0, pdu_size);
                }
            }
        }


        public string Name { get; internal set; }
        private RpcConfig rpc_config;
        protected byte[] buffer = null;
        protected System.Object lockObj = new System.Object();
        private string mqtt_topic;

        public string GetName()
        {
            return Name;
        }

        private void InitUdpServer()
        {
            map.Add(this.rpc_config.asset_name + ":" + this.rpc_config.channel_id, this);

            if (RpcReader.isUdpActive == false)
            {
                RpcReader.StartUdpServer();
            }

            var result = RpcClient.SubscribePduChannel(
                AssetConfigLoader.core_config.cpp_asset_name,
                rpc_config.asset_name,
                rpc_config.channel_id,
                rpc_config.PduSize,
                AssetConfigLoader.core_config.asset_ipaddr,
                AssetConfigLoader.core_config.pdu_udp_portno_asset,
                rpc_config.get_method_type());
            if (result == false)
            {
                throw new InvalidOperationException("RPC ERROR UDP");
            }
        }
        private void InitMqttServer()
        {
            this.mqtt_topic = "hako_mqtt_" + this.rpc_config.channel_id;
            HakoMqtt.AddSubscribeTopic(this.mqtt_topic);
            var result = RpcClient.SubscribePduChannel(
                AssetConfigLoader.core_config.cpp_asset_name,
                rpc_config.asset_name,
                rpc_config.channel_id,
                rpc_config.PduSize,
                AssetConfigLoader.core_config.asset_ipaddr,
                AssetConfigLoader.core_config.pdu_udp_portno_asset,
                rpc_config.get_method_type());
            if (result == false)
            {
                throw new InvalidOperationException("RPC ERROR MQTT");
            }
        }
        public void Initialize(IIoReaderConfig config)
        {
            this.rpc_config = config as RpcConfig;
            SimpleLogger.Get().Log(Level.INFO, "RpcReader: channel_id=" + this.rpc_config.channel_id);
            SimpleLogger.Get().Log(Level.INFO, "RpcReader: method_type=" + this.rpc_config.get_method_type());
            if (this.rpc_config.get_method_type() == "UDP")
            {
                this.InitUdpServer();
            }
            else
            {
                this.InitMqttServer();
            }

        }

        private IPduCommData RecvUdp(string io_key)
        {
            lock (this.lockObj)
            {
                if (this.buffer == null)
                {
                    return null;
                }
                else
                {
                    byte[] tmp_buf = new byte[this.buffer.Length];
                    Buffer.BlockCopy(this.buffer, 0, tmp_buf, 0, buffer.Length);
                    //SimpleLogger.Get().Log(Level.DEBUG, "recv:" + buffer.Length);
                    return new PduCommBinaryData(tmp_buf);
                }
            }
        }
        private IPduCommData RecvMqtt(string io_key)
        {
            var data = HakoMqtt.GetData(this.mqtt_topic);
            if (data == null)
            {
                return null;
            }
            else {
                byte[] tmp_buf = new byte[data.Length];
                Buffer.BlockCopy(data, 0, tmp_buf, 0, data.Length);
                return new PduCommBinaryData(tmp_buf);
            }
        }

        public IPduCommData Recv(string io_key)
        {
            if (this.rpc_config.get_method_type() == "UDP")
            {
                return RecvUdp(io_key);
            }
            else
            {
                return RecvMqtt(io_key);
            }
        }

    }
}
#endif