using Hakoniwa.Core;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Shm
{
    class ShmReader: IIoReader
    {
        public string Name { get; internal set; }
        private ShmConfig shm_config;

        public string GetName()
        {
            return Name;
        }

        public void Initialize(IIoReaderConfig config)
        {
            this.shm_config = config as ShmConfig;
        }

        public IPduCommData Recv(string io_key)
        {
            IntPtr buffer = Marshal.AllocHGlobal(shm_config.io_size);
            bool ret = HakoCppWrapper.asset_read_pdu(this.shm_config.asset_name, shm_config.channel_id, buffer, (uint)shm_config.io_size);
            if (ret == false)
            {
                throw new ArgumentException("Can not read pdul!! " + this.shm_config.asset_name + " channel_id=" + this.shm_config.channel_id);
            }
            var arg_buffer = new byte[shm_config.io_size];
            Marshal.Copy(buffer, arg_buffer, 0, arg_buffer.Length);
            return new PduCommBinaryData(arg_buffer);
        }

    }
}
