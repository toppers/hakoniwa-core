using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Micon
{
    public class SimpleMiconPduReaderRawConverter : IPduReaderConverter
    {
        private byte[] my_buffer = new byte[8];

        public SimpleMiconPduReaderRawConverter()
        {

        }
        public IPduCommData ConvertToIoData(IPduReader src)
        {
            if (!src.IsValidData())
            {
                return null;
            }
            byte[] tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataInt64("simtime"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 8, tmp_bytes.Length);
            return new PduCommBinaryData(this.my_buffer);
        }

        public void ConvertToPduData(IPduCommData src, IPduReader dst)
        {
            PduCommBinaryData src_data = src as PduCommBinaryData;
            byte[] buffer = src_data.GetData();
            dst.GetWriteOps().SetData("simtime", BitConverter.ToInt64(buffer, 8));
        }
    }
}
