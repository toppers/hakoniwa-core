using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Mmap
{
    public class MmapConfig : IIoReaderConfig, IIoWriterConfig
    {
        public string filepath;
        public int io_size;
    }
}
