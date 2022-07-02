﻿using Hakoniwa.Core;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Shm
{
    class ShmWriter : IIoWriter
    {
        public string Name { get; internal set; }
        private ShmConfig shm_config;

        public void Flush(IPduCommData data)
        {
            PduCommBinaryData binary = null;

            if (data is PduCommBinaryData)
            {
                binary = (PduCommBinaryData)data;
            }
            if (data == null)
            {
                throw new ArgumentException("Invalid data type");
            }
            byte[] buf = binary.GetData();
            if (buf.Length != this.shm_config.io_size)
            {
                throw new ArgumentException("Invalid io_size:" + buf.Length);
            }
            int size = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr buffer = Marshal.AllocHGlobal(size);

            bool ret = HakoCppWrapper.asset_write_pdu(this.shm_config.asset_name, shm_config.channel_id, buffer, (uint)shm_config.io_size);
            if (ret == false)
            {
                throw new ArgumentException("Can not write pdul!! " + this.shm_config.asset_name + " channel_id=" + this.shm_config.channel_id);
            }
        }

        public string GetName()
        {
            return Name;
        }

        public void Initialize(IIoWriterConfig config)
        {
            this.shm_config = config as ShmConfig;
            bool ret = HakoCppWrapper.asset_create_pdu_channel(this.shm_config.channel_id, (uint)this.shm_config.io_size);
            if (ret == false)
            {
                throw new ArgumentException("Can not create pdu channel!! channel_id=" + this.shm_config.channel_id);
            }
        }
    }
}
