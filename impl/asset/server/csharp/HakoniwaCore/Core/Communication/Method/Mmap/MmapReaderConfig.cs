using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Method.Mmap
{
    public class MmapReaderConfig : IIoReaderConfig
    {
        public string filepath;
        public int io_size;
    }
}
