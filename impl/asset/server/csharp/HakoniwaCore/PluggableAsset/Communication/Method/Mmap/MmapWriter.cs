using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using Hakoniwa.Core.Utils.Logger;

namespace Hakoniwa.PluggableAsset.Communication.Method.Mmap
{
    class MmapWriter : IIoWriter
    {
        private MmapConfig mmap_config;
        private MemoryMappedFile mappedFile;
        private UnmanagedMemoryAccessor accessor;

        public string Name { get; internal set; }

        public string GetName()
        {
            return this.Name;
        }
        public void Flush(ref byte[] buf)
        {
            SimpleLogger.Get().Log(Level.INFO, "MmapWrite:file=" + mmap_config.filepath + " len=" + buf.Length);
            accessor.WriteArray<byte>(0, buf, 0, buf.Length);
        }

        public void Initialize(IIoWriterConfig config)
        {
            mmap_config = config as MmapConfig;
            if (!System.IO.File.Exists(mmap_config.filepath))
            {
                throw new InvalidOperationException("filepath is invalid:" + mmap_config.filepath);
            }
            this.mappedFile = MemoryMappedFile.CreateFromFile(mmap_config.filepath, System.IO.FileMode.Open);
            this.accessor = mappedFile.CreateViewAccessor();
            byte init_data = 0;
            for (int i = 0; i < mmap_config.io_size; i++)
            {
                accessor.Write<byte>(i, ref init_data);
            }
        }

    }
}
