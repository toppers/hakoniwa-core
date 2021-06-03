using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;


namespace Hakoniwa.PluggableAsset.Communication.Method.Mmap
{
    class MmapReader : IIoReader
    {
        private MmapConfig mmap_config;
        private MemoryMappedFile mappedFile;
        private UnmanagedMemoryAccessor accessor;

        public string Name { get; internal set; }

        public string GetName()
        {
            return this.Name;
        }

        public void Initialize(IIoReaderConfig config)
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

        public IPduCommData Recv()
        {
            byte[] buffer = new byte[this.mmap_config.io_size];
            this.accessor.ReadArray<byte>(0, buffer, 0, buffer.Length);
            return new PduCommBinaryData(buffer);
        }
    }
}
