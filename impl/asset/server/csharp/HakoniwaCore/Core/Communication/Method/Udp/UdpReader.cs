using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hakoniwa.Core.Communication.Method.Udp
{
    class UdpReader : IIoReader
    {
        UdpReaderConfig udp_config = null;
        private Object lockObj = new Object();
        private byte[] buffer = null;
        private Thread thread;
        public void Initialize(IIoReaderConfig config)
        {
            udp_config = config as UdpReaderConfig;
            this.buffer = new byte[udp_config.io_size];
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }

        public void Recv(ref byte[] buf)
        {
            lock (this.lockObj)
            {
                Buffer.BlockCopy(this.buffer, 0, buf, 0, this.buffer.Length);
            }
        }

        private void ThreadMethod()
        {
            UdpClient client;
            client = new UdpClient(udp_config.ipaddr, udp_config.portno);
            while (true)
            {
                IPEndPoint remoteEP = null;
                byte[] data = client.Receive(ref remoteEP);
                if (data.Length != buffer.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                lock (this.lockObj)
                {
                    Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
                }
            }
        }
    }
}
