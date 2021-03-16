using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Hakoniwa.Core.Communication.Method.Udp
{
    class UdpWriter : IIoWriter
    {
        UdpWriterConfig udp_config = null;
        private UdpClient client;
        public void Flush(ref byte[] buf)
        {
            client.Send(buf, buf.Length);
        }

        public void Initialize(IIoWriterConfig config)
        {
            udp_config = config as UdpWriterConfig;
            client = new UdpClient();
            client.Connect(udp_config.ipaddr, udp_config.portno);
            return;
        }

    }
}
