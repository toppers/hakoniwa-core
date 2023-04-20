using Hakoniwa.Core;
using Hakoniwa.Core.Utils.Logger;
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
        private IntPtr buffer;
        private int buffer_size;
        private ShmConfig shm_config;
        private string asset_name;

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
            Marshal.Copy(buf, 0, buffer, buffer_size);
            bool ret = HakoCppWrapper.asset_write_pdu(this.asset_name, this.shm_config.asset_name, shm_config.channel_id, buffer, (uint)shm_config.io_size);
            if (ret == false)
            {
                SimpleLogger.Get().Log(Level.INFO, "this.asset_name=" + this.asset_name);
                SimpleLogger.Get().Log(Level.INFO, "shm_config.asset_name=" + this.shm_config.asset_name);
                SimpleLogger.Get().Log(Level.INFO, "channel_id=" + this.shm_config.channel_id);
                SimpleLogger.Get().Log(Level.INFO, "io_size=" + this.shm_config.io_size);
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
            this.asset_name = AssetConfigLoader.core_config.cpp_asset_name;
            byte[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            buffer_size = Marshal.SizeOf(array[0]) * shm_config.io_size;
            this.buffer = Marshal.AllocHGlobal(buffer_size);
            SimpleLogger.Get().Log(Level.INFO, "this.asset_name=" + this.asset_name);
            SimpleLogger.Get().Log(Level.INFO, "shm_config.asset_name=" + this.shm_config.asset_name);
            SimpleLogger.Get().Log(Level.INFO, "channel_id=" + this.shm_config.channel_id);
            SimpleLogger.Get().Log(Level.INFO, "io_size=" + this.shm_config.io_size);
            bool ret = HakoCppWrapper.asset_create_pdu_lchannel(this.shm_config.asset_name, this.shm_config.channel_id, (uint)shm_config.io_size);
            if (ret == false)
            {
                throw new ArgumentException("Can not create pdu channel!! channel_id=" + this.shm_config.channel_id);
            }
        }
    }
}
