using Hakoniwa.Core;
using Hakoniwa.Core.Rpc;
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
        private static Dictionary<int, RpcReader> map = new Dictionary<int, RpcReader>();
        private static Thread thread;
        private static bool isActive = false;
        public static void StartServer()
        {
            if (isActive)
            {
                return;
            }
            isActive = true;
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }
        private static void ThreadMethod()
        {
            UdpClient client;
            client = new UdpClient(AssetConfigLoader.core_config.pdu_udp_portno_asset);
            while (true)
            {
                IPEndPoint remoteEP = null;
                byte[] data = client.Receive(ref remoteEP);
                int channel_id = BitConverter.ToInt32(data, 0);
                int pdu_size = BitConverter.ToInt32(data, 4);
                var obj = map[channel_id];
                lock (obj.lockObj)
                {
                    if (obj.buffer == null)
                    {
                        obj.buffer = new byte[pdu_size];
                    }
                    Buffer.BlockCopy(data, 8, obj.buffer, 0, pdu_size);
                }
            }
        }


        public string Name { get; internal set; }
        private RpcConfig rpc_config;
        protected byte[] buffer = null;
        protected System.Object lockObj = new System.Object();
       
        public string GetName()
        {
            return Name;
        }

        public void Initialize(IIoReaderConfig config)
        {
            this.rpc_config = config as RpcConfig;
            map.Add(this.rpc_config.channel_id, this);

            if (RpcReader.isActive == false)
            {
                RpcReader.StartServer();
            }

            var result = RpcClient.SubscribePduChannel(
                rpc_config.asset_name, 
                rpc_config.channel_id, 
                rpc_config.PduSize,
                AssetConfigLoader.core_config.asset_ipaddr,
                AssetConfigLoader.core_config.pdu_udp_portno_asset);
            if (result == false)
            {
                throw new InvalidOperationException("RPC ERROR");
            }
        }

        public IPduCommData Recv(string io_key)
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

    }
}
