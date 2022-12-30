using Hakoniwa.Core;
using Hakoniwa.Core.Rpc;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Rpc
{
    class RpcWriter : IIoWriter
    {
        private int portno;
        public string Name { get; internal set; }
        private byte[] buffer = null;
        private RpcConfig rpc_config;
        private UdpClient client;

        public void Flush(IPduCommData data)
        {
            PduCommBinaryData binary = null;

            if (data is PduCommBinaryData)
            {
                binary = (PduCommBinaryData)data;
            }
            if (data == null)
            {
                return;
            }
            byte[] buf = binary.GetData();
            //Debug.Log("UdpSend:" + buf.Length);
            //SimpleLogger.Get().Log(Level.DEBUG, "flush channel_id=" + rpc_config.channel_id +" port="+ this.portno + " len=" + buf.Length);
            try
            {
                Buffer.BlockCopy(buf, 0, this.buffer, 8, buf.Length);
#if false
                for (int i = 0; i < buf.Length; i++)
                {
                    SimpleLogger.Get().Log(Level.INFO, "value[" + i + "] = " + this.buffer[i]);
                }
#endif
                client.Send(this.buffer, this.buffer.Length);
            }
            catch (Exception)
            {
            }
        }

        public string GetName()
        {
            return Name;
        }

        public void Initialize(IIoWriterConfig config)
        {
            this.rpc_config = config as RpcConfig;
            this.buffer = new byte[rpc_config.PduSize + 8];
            //chanel_id
            var tmp_bytes = BitConverter.GetBytes(rpc_config.channel_id);
            Buffer.BlockCopy(tmp_bytes, 0, this.buffer, 0, tmp_bytes.Length);
            //pdu_size
            tmp_bytes = BitConverter.GetBytes(rpc_config.PduSize);
            Buffer.BlockCopy(tmp_bytes, 0, this.buffer, 4, tmp_bytes.Length);

            this.portno = RpcClient.CreatePduChannel(rpc_config.asset_name, rpc_config.channel_id, rpc_config.PduSize);
            if (this.portno < 0)
            {
                throw new InvalidOperationException("RPC ERROR");
            }
            client = new UdpClient();
            client.Connect(AssetConfigLoader.core_config.core_ipaddr, this.portno);
        }
    }
}
