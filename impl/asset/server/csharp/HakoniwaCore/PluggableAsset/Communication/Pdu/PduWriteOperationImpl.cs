using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    class PduWriteOperationImpl : IPduWriteOperation
    {
        private ObsoletePdu pdu;
        private PduConfig pdu_config;
        private byte[] buffer;
        public PduWriteOperationImpl(ObsoletePdu arg_pdu)
        {
            this.pdu = arg_pdu;
            this.pdu_config = this.pdu.GetConfig();
            this.buffer = this.pdu.GetBuffer();
        }
        public void SetData(string field_name, byte[] value)
        {
            Buffer.BlockCopy(value, 0, this.buffer, pdu_config.GetOffset(field_name), value.Length);
        }

        public void SetData(string field_name, UInt32 value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }
        public void SetData(string field_name, Int32 value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }
        public void SetData(string field_name, UInt64 value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        public void SetHeaderData(string field_name, Int64 value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }
        public void SetData(string field_name, double value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        public void SetData(string field_name, byte value)
        {
            this.buffer[pdu_config.GetOffset(field_name)] = value;
        }

        public void SetData(string field_name, sbyte value)
        {
            this.buffer[pdu_config.GetOffset(field_name)] = (byte)value;
        }

        public void SetData(string field_name, ushort value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }

        public void SetData(string field_name, short value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }

        public void SetData(string field_name, long value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }
        public void SetHeaderData(string field_name, Int32 value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }
        public void SetHeaderData(string field_name, string value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = System.Text.Encoding.ASCII.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }

        public void SetData(string field_name, float value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        public void SetData(string field_name, string value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, sbyte[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, ushort[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, short[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, uint[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, int[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, ulong[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, long[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, double[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, float[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, string[] value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, Pdu pdu)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, Pdu[] pdu)
        {
            throw new NotImplementedException();
        }
    }
}
