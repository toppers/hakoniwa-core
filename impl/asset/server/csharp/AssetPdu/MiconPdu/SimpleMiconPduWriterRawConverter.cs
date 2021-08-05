using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Micon
{
    public class SimpleMiconPduWriterRawConverter : IPduWriterConverter
    {
        private byte[] my_buffer = new byte[8];
        public IPduCommData ConvertToIoData(IPduWriter src)
        {
            //int64 hakoniwa_time
            var tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataInt64("simtime"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 0, tmp_bytes.Length);
            var obj = new PduCommBinaryData(this.my_buffer);
            return obj;
        }
    }
}
