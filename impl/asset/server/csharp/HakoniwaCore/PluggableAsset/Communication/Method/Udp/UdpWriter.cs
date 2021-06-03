using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Udp
{
    class UdpWriter : IIoWriter
    {
        UdpConfig udp_config = null;
        private UdpClient client;

        public string Name { get; internal set; }

        public string GetName()
        {
            return this.Name;
        }
        public void Flush(IPduCommData data)
        {
            PduCommBinaryData binary = null;

            if (data is PduCommBinaryData)
            {
                binary = (PduCommBinaryData)data;
            }
            if (data == null)
            {
                throw new ArgumentException("Invalid data type");
            }
            byte[] buf = binary.GetData();
            //Debug.Log("UdpSend:" + buf.Length);
            //SimpleLogger.Get().Log(Level.DEBUG, "flush ipaddr=" + udp_config.IpAddr +" port="+ udp_config.Portno + " len=" + buf.Length);
            try
            {
                client.Send(buf, buf.Length);
            }
            catch (Exception)
            {
            }
        }

        public void Initialize(IIoWriterConfig config)
        {
            udp_config = config as UdpConfig;
            client = new UdpClient();
            client.Connect(udp_config.IpAddr, udp_config.Portno);
            return;
        }

    }
}
