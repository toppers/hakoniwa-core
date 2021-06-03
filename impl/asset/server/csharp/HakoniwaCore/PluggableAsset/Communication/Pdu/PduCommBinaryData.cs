using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    class PduCommBinaryData : IPduCommData
    {
        private byte[] buffer;

        public PduCommBinaryData(byte[] data)
        {
            this.buffer = data;
        }

        public byte[] GetData()
        {
            return this.buffer;
        }
    }
}
