using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Hakoniwa.Core.Communication.Method.Mmap
{
    class MmapWriter : IIoWriter
    {
        private byte[] buffer;
        private MmapWriterConfig mmap_config;
        private MemoryMappedFile mappedFile;
        private UnmanagedMemoryAccessor accessor;
        public void Flush()
        {
            accessor.WriteArray<byte>(0, buffer, 0, buffer.Length);
        }

        public void Initialize(IIoWriterConfig config)
        {
            mmap_config = config as MmapWriterConfig;
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
        public void SetBuffer(ref byte[] buf)
        {
            this.buffer = buf;
        }

    }
}
