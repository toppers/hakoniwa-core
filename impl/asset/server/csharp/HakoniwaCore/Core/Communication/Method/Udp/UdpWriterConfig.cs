using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Method.Udp
{
    class UdpWriterConfig : IIoWriterConfig
    {
        public string ipaddr;
        public int portno;
        public int io_size;
    }
}
