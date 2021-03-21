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
        public void Flush(ref byte[] buf)
        {
            //Debug.Log("UdpSend:" + buf.Length);
            client.Send(buf, buf.Length);
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
