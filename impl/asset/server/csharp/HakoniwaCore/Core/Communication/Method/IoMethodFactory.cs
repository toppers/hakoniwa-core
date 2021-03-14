using Hakoniwa.Core.Communication.Method.Mmap;
using Hakoniwa.Core.Communication.Method.Udp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Method
{
    class IoMethodFactory
    {
        public static IIoReader create(IIoReaderConfig config)
        {
            if (config is UdpReaderConfig)
            {
                UdpReader reader = new UdpReader();
                reader.Initialize(config);
                return reader;
            }
            else if (config is MmapWriterConfig)
            {
                MmapReader reader = new MmapReader();
                reader.Initialize(config);
                return reader;
            }
            else
            {
                return null;
            }
        }
        public static IIoWriter create(IIoWriterConfig config)
        {
            if (config is UdpWriterConfig) {
                UdpWriter writer = new UdpWriter();
                writer.Initialize(config);
                return writer;
            }
            else if (config is MmapWriterConfig)
            {
                MmapWriter writer = new MmapWriter();
                writer.Initialize(config);
                return writer;
            }
            else
            {
                return null;
            }
        }
    }
}
