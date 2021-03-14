using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Method.Udp
{
    public class UdpReaderConfig : IIoReaderConfig
    {
        public string ipaddr;
        public int portno;
        public int io_size;
    }
}
