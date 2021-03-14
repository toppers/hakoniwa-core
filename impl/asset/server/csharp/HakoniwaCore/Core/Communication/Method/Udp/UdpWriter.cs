using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Hakoniwa.Core.Communication.Method.Udp
{
    class UdpWriter : IIoWriter
    {
        UdpWriterConfig udp_config = null;
        private byte[] buffer = null;
        private UdpClient client;
        public void Flush()
        {
            client.Send(this.buffer, this.buffer.Length);
        }

        public void Initialize(IIoWriterConfig config)
        {
            udp_config = config as UdpWriterConfig;
            client = new UdpClient();
            client.Connect(udp_config.ipaddr, udp_config.portno);
            return;
        }

        public void SetBuffer(ref byte[] buf)
        {
            this.buffer = buf;
        }
    }
}
