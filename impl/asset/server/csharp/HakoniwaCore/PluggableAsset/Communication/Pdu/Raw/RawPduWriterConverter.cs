using System;
using Hakoniwa.Core.Utils.Logger;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Raw
{
    class RawPduWriterConverter : IPduWriterConverter
    {
        public static IPduCommData ConvertToIoData(IPduReadOperation src)
        {
            string type_name = src.Ref(null).GetName();
            var off_info = PduOffset.Get(type_name);
            if (off_info == null)
            {
                throw new InvalidOperationException("Error: Can not found offset: type=" + type_name);
            }
            byte[] buffer = new byte[off_info.size];
            ConvertFromStruct(off_info, 0, buffer, src);

            var obj = new PduCommBinaryData(buffer);
            return obj;
        }

        private static int ConvertFromStruct(PduBinOffsetInfo off_info, int off, byte[] dst_buffer, IPduReadOperation src)
        {
            int next_off = off;
            SimpleLogger.Get().Log(Level.INFO, "TO BIN:Start Convert: package=" + off_info.package_name + " type=" + off_info.type_name);
            foreach (var elm in off_info.elms)
            {
                if (elm.is_primitive)
                {
                    //primitive
                    if (elm.is_array)
                    {
                        next_off = ConvertFromPrimtiveArray(elm, next_off, dst_buffer, src);
                    }
                    else
                    {
                        next_off = ConvertFromPrimtive(elm, next_off, dst_buffer, src);
                    }
                }
                else
                {
                    //struct
                    if (elm.is_array)
                    {
                        next_off = ConvertFromStructArray(elm, next_off, dst_buffer, src);
                    }
                    else
                    {
                        PduBinOffsetInfo struct_off_info = PduOffset.Get(elm.type_name);
                        next_off = ConvertFromStruct(struct_off_info, next_off, dst_buffer, src.Ref(elm.field_name).GetPduReadOps());
                    }
                }
            }
            return next_off;
        }

        private static int ConvertFromStructArray(PduBinOffsetElmInfo elm, int off, byte[] dst_buffer, IPduReadOperation src)
        {
            int next_off = off;
            PduBinOffsetInfo struct_off_info = PduOffset.Get(elm.type_name);
            for (int i = 0; i < elm.array_size; i++)
            {
                Pdu src_data = src.Refs(elm.field_name)[i];
                next_off = ConvertFromStruct(struct_off_info, next_off, dst_buffer, src_data.GetPduReadOps());
            }
            return next_off;
        }

        private static int ConvertFromPrimtiveArray(PduBinOffsetElmInfo elm, int off, byte[] dst_buffer, IPduReadOperation src)
        {
            byte[] tmp_bytes = null;
            for (int i = 0; i < elm.array_size; i++)
            {
                switch (elm.type_name)
                {
                    case "int8":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataInt8Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "int16":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataInt16Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "int32":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataInt32Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "int64":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataInt64Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "uint8":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataUInt8Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "uint16":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataUInt16Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "uint32":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataUInt32Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "uint64":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataUInt64Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "float32":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataFloat32Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "float64":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataFloat64Array(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "bool":
                        tmp_bytes = BitConverter.GetBytes(src.GetDataBoolArray(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    case "string":
                        tmp_bytes = System.Text.Encoding.ASCII.GetBytes(src.GetDataStringArray(elm.field_name)[i]);
                        Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, (off + i * elm.elm_size), tmp_bytes.Length);
                        break;
                    default:
                        throw new InvalidCastException("Error: Can not found ptype: " + elm.type_name);
                }
            }
            return (off + (elm.elm_size * elm.array_size));
        }

        private static int ConvertFromPrimtive(PduBinOffsetElmInfo elm, int off, byte[] dst_buffer, IPduReadOperation src)
        {
            byte[] tmp_bytes = null;
            switch (elm.type_name)
            {
                case "int8":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataInt8(elm.field_name));
                    break;
                case "int16":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataInt16(elm.field_name));
                    break;
                case "int32":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataInt32(elm.field_name));
                    break;
                case "int64":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataInt64(elm.field_name));
                    break;
                case "uint8":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataUInt8(elm.field_name));
                    break;
                case "uint16":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataUInt16(elm.field_name));
                    break;
                case "uint32":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataUInt32(elm.field_name));
                    break;
                case "uint64":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataUInt64(elm.field_name));
                    break;
                case "float32":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataFloat32(elm.field_name));
                    break;
                case "float64":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataFloat64(elm.field_name));
                    break;
                case "bool":
                    tmp_bytes = BitConverter.GetBytes(src.GetDataBool(elm.field_name));
                    break;
                case "string":
                    tmp_bytes = System.Text.Encoding.ASCII.GetBytes(src.GetDataString(elm.field_name));
                    break;
                default:
                    throw new InvalidCastException("Error: Can not found ptype: " + elm.type_name);
            }
            Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, off, tmp_bytes.Length);
            return (off + elm.elm_size);
        }

        public IPduCommData ConvertToIoData(IPduWriter src)
        {
            return ConvertToIoData(src.GetReadOps());
        }
    }
}
