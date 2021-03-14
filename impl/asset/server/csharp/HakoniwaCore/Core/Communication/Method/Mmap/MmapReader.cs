﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;


namespace Hakoniwa.Core.Communication.Method.Mmap
{
    class MmapReader : IIoReader
    {
        private MmapReaderConfig mmap_config;
        private MemoryMappedFile mappedFile;
        private UnmanagedMemoryAccessor accessor;

        public void Initialize(IIoReaderConfig config)
        {
            mmap_config = config as MmapReaderConfig;
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

        public void Recv(ref byte[] buf)
        {
            accessor.ReadArray<byte>(0, buf, 0, buf.Length);
        }
    }
}