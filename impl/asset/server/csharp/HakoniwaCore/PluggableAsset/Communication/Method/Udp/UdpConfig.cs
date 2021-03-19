using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Udp
{
    public class UdpConfig : IIoWriterConfig, IIoReaderConfig
    {
        private string ipaddr;
        private int portno;
        private int io_size;
        public string IpAddr
        {
            set { this.ipaddr = value; }
            get { return this.ipaddr; }
        }
        public int Portno
        {
            set { this.portno = value; }
            get { return this.portno;  }
        }
        public int IoSize
        {
            set { this.io_size = value; }
            get { return this.io_size;  }
        }
    }
}
