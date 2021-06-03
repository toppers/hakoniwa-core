using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    class PduReadOperationImpl : IPduReadOperation
    {
        private Pdu pdu;
        private PduConfig pdu_config;
        private byte[] buffer;
        public PduReadOperationImpl(Pdu arg_pdu)
        {
            this.pdu = arg_pdu;
            this.pdu_config = this.pdu.GetConfig();
            this.buffer = this.pdu.GetBuffer();
        }
        public Int64 GetHeaderData(string field_name)
        {
            return BitConverter.ToInt64(this.buffer, pdu_config.GetHeaderOffset(field_name));
        }

        public Int32 GetDataInt32(string field_name)
        {
            return BitConverter.ToInt32(this.buffer, pdu_config.GetOffset(field_name));
        }
        public UInt32 GetDataUInt32(string field_name)
        {
            return BitConverter.ToUInt32(this.buffer, pdu_config.GetOffset(field_name));
        }
        public UInt64 GetDataUInt64(string field_name)
        {
            return BitConverter.ToUInt64(this.buffer, pdu_config.GetOffset(field_name));
        }
        public Int64 GetDataInt64(string field_name)
        {
            return BitConverter.ToInt64(this.buffer, pdu_config.GetOffset(field_name));
        }

        public double GetDataDouble(string field_name)
        {
            return BitConverter.ToDouble(this.buffer, pdu_config.GetOffset(field_name));
        }

        public byte[] GetDataBytes(string field_name)
        {
            if (field_name != null)
            {
                byte[] tmp_buf = new byte[this.pdu_config.GetSize(field_name)];
                Buffer.BlockCopy(this.buffer, pdu_config.GetOffset(field_name), tmp_buf, 0, tmp_buf.Length);
                return tmp_buf;
            }
            else
            {
                return this.buffer;
            }
        }

        public sbyte GetDataInt8(string field_name)
        {
            return (sbyte)this.buffer[pdu_config.GetOffset(field_name)];
        }

        public byte GetDataUInt8(string field_name)
        {
            return this.buffer[pdu_config.GetOffset(field_name)];
        }

        public short GetDataInt16(string field_name)
        {
            return BitConverter.ToInt16(this.buffer, pdu_config.GetOffset(field_name));
        }

        public ushort GetDataUInt16(string field_name)
        {
            return BitConverter.ToUInt16(this.buffer, pdu_config.GetOffset(field_name));
        }
    }
}
