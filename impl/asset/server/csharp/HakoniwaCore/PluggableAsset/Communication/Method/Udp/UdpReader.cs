using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hakoniwa.PluggableAsset.Communication.Method.Udp
{
    class UdpReader : IIoReader
    {
        UdpConfig udp_config = null;
        private System.Object lockObj = new System.Object();
        private byte[] buffer = null;
        private Thread thread;

        public string Name { get; internal set; }

        public string GetName()
        {
            return this.Name;
        }

        public void Initialize(IIoReaderConfig config)
        {
            udp_config = config as UdpConfig;
            this.buffer = new byte[udp_config.IoSize];
            Buffer.SetByte(this.buffer, 0, 0);
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
            //client = new UdpClient(udp_config.IpAddr, udp_config.Portno);
            client = new UdpClient(udp_config.Portno);
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
