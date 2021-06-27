using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Communication.Pdu;
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
            //this.buffer = new byte[udp_config.IoSize];
            //Buffer.SetByte(this.buffer, 0, 0);
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
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

        private void ThreadMethod()
        {
            UdpClient client;
            //client = new UdpClient(udp_config.IpAddr, udp_config.Portno);
            client = new UdpClient(udp_config.Portno);
            while (true)
            {
                IPEndPoint remoteEP = null;
                byte[] data = client.Receive(ref remoteEP);
                lock (this.lockObj)
                {
                    //Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
                    this.buffer = data;
                }
            }
        }
    }
}
