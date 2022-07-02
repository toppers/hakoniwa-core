using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Shm
{
    class ShmConfig : IIoReaderConfig, IIoWriterConfig
    {
        public StringBuilder asset_name;
        public int channel_id;
        public int io_size;
    }
}
