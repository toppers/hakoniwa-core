#if NO_USE_GRPC
#else
using Hakoniwa.Core;
using Hakoniwa.Core.Rpc;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Communication.Method.Mqtt;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hakoniwa.PluggableAsset.Communication.Method.Rpc
{
    class RpcWriter : IIoWriter
    {
        private int portno;
        public string Name { get; internal set; }
        private byte[] buffer = null;
        private RpcConfig rpc_config;
        private UdpClient client;
        private string mqtt_topic = null;
        private int count = 0;
        private int header_off = 0;

        private void FlushUdp(PduCommBinaryData binary)
        {
            byte[] buf = binary.GetData();
            //Debug.Log("UdpSend:" + buf.Length);
            //SimpleLogger.Get().Log(Level.DEBUG, "flush channel_id=" + rpc_config.channel_id +" port="+ this.portno + " len=" + buf.Length);
            try
            {
                Buffer.BlockCopy(buf, 0, this.buffer, this.header_off, buf.Length);
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
        private async Task FlushMqtt(PduCommBinaryData binary)
        {
            await HakoMqtt.SendMessage(this.mqtt_topic, binary.GetData());
        }
        public async void Flush(IPduCommData data)
        {
            this.count++;
            if ((this.count % this.rpc_config.write_count) != 0)
            {
                return;
            }
            this.count = 0;
            PduCommBinaryData binary = null;

            if (data is PduCommBinaryData)
            {
                binary = (PduCommBinaryData)data;
            }
            if (data == null)
            {
                //書き込み周期でない場合はnullが入って来る
                return;
            }
            if (this.rpc_config.get_method_type().Equals("UDP"))
            {
                this.FlushUdp(binary);
            }
            else
            {
                await this.FlushMqtt(binary);
            }
        }

        public string GetName()
        {
            return Name;
        }

        public async void Initialize(IIoWriterConfig config)
        {
            this.rpc_config = config as RpcConfig;
            if (this.rpc_config.get_method_type().Equals("UDP"))
            {
                this.buffer = new byte[rpc_config.PduSize + 8 + 4 + 256 /* for asset name */ ];
                //chanel_id
                var tmp_bytes = BitConverter.GetBytes(rpc_config.channel_id);
                Buffer.BlockCopy(tmp_bytes, 0, this.buffer, 0, tmp_bytes.Length);
                //pdu_size
                tmp_bytes = BitConverter.GetBytes(rpc_config.PduSize);
                Buffer.BlockCopy(tmp_bytes, 0, this.buffer, 4, tmp_bytes.Length);
                //asset_name
                tmp_bytes = System.Text.Encoding.ASCII.GetBytes(rpc_config.asset_name);
                var len_bytes = BitConverter.GetBytes(tmp_bytes.Length);
                Buffer.BlockCopy(len_bytes, 0, this.buffer, 8, len_bytes.Length);
                Buffer.BlockCopy(tmp_bytes, 0, this.buffer, (8 + 4), tmp_bytes.Length);
                this.header_off = 8 + 4 + tmp_bytes.Length;
                this.portno = RpcClient.CreatePduChannel(AssetConfigLoader.core_config.cpp_asset_name, rpc_config.asset_name, rpc_config.channel_id, rpc_config.PduSize, rpc_config.get_method_type());
                if (this.portno < 0)
                {
                    throw new InvalidOperationException("RPC UDP ERROR portno=" + portno + " channel=" + rpc_config.channel_id);
                }
                client = new UdpClient();
                client.Connect(AssetConfigLoader.core_config.core_ipaddr, this.portno);
            }
            else
            {
                //mqtt
                this.mqtt_topic = "hako_mqtt_" + this.rpc_config.asset_name + "_" + this.rpc_config.channel_id;
                this.buffer = new byte[rpc_config.PduSize];
                this.portno = RpcClient.CreatePduChannel(AssetConfigLoader.core_config.cpp_asset_name, rpc_config.asset_name, rpc_config.channel_id, rpc_config.PduSize, rpc_config.get_method_type());
                if (this.portno < 0)
                {
                    throw new InvalidOperationException("RPC MQTT ERROR");
                }
                await HakoMqtt.Connect(AssetConfigLoader.core_config.core_ipaddr, this.portno);
            }

        }
    }
}
#endif