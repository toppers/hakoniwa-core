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

        private static void ConvertFromStruct(PduBinOffsetInfo off_info, int base_off, byte[] dst_buffer, IPduReadOperation src)
        {
            //SimpleLogger.Get().Log(Level.INFO, "TO BIN:Start Convert: package=" + off_info.package_name + " type=" + off_info.type_name);
            foreach (var elm in off_info.elms)
            {
                if (elm.is_primitive)
                {
                    //primitive
                    if (elm.is_array)
                    {
                        ConvertFromPrimtiveArray(elm, base_off, dst_buffer, src);
                    }
                    else
                    {
                        ConvertFromPrimtive(elm, base_off, dst_buffer, src);
                    }
                }
                else
                {
                    //struct
                    if (elm.is_array)
                    {
                        ConvertFromStructArray(elm, base_off + elm.offset, dst_buffer, src);
                    }
                    else
                    {
                        PduBinOffsetInfo struct_off_info = PduOffset.Get(elm.type_name);
                        ConvertFromStruct(struct_off_info, base_off + elm.offset, dst_buffer, src.Ref(elm.field_name).GetPduReadOps());
                    }
                }
            }
        }

        private static void ConvertFromStructArray(PduBinOffsetElmInfo elm, int base_off, byte[] dst_buffer, IPduReadOperation src)
        {
            PduBinOffsetInfo struct_off_info = PduOffset.Get(elm.type_name);
            for (int i = 0; i < elm.array_size; i++)
            {
                Pdu src_data = src.Refs(elm.field_name)[i];
                ConvertFromStruct(struct_off_info, base_off + (i * elm.elm_size), dst_buffer, src_data.GetPduReadOps());
            }
        }

        private static void ConvertFromPrimtiveArray(PduBinOffsetElmInfo elm, int base_off, byte[] dst_buffer, IPduReadOperation src)
        {
            byte[] tmp_bytes = null;
            int off = base_off + elm.offset;
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
        }

        private static void ConvertFromPrimtive(PduBinOffsetElmInfo elm, int off, byte[] dst_buffer, IPduReadOperation src)
        {
            byte[] tmp_bytes = null;
            switch (elm.type_name)
            {
                case "int8":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataInt8(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataInt8(elm.field_name));
                    break;
                case "int16":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataInt16(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataInt16(elm.field_name));
                    break;
                case "int32":
                    //SimpleLogger.Get().Log(Level.INFO, off + ":" + elm.field_name + " = " + src.GetDataInt32(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataInt32(elm.field_name));
                    break;
                case "int64":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataInt64(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataInt64(elm.field_name));
                    break;
                case "uint8":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataUInt8(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataUInt8(elm.field_name));
                    break;
                case "uint16":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataUInt16(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataUInt16(elm.field_name));
                    break;
                case "uint32":
                    //SimpleLogger.Get().Log(Level.INFO, off + ":" + elm.field_name + " = " + src.GetDataUInt32(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataUInt32(elm.field_name));
                    break;
                case "uint64":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataUInt64(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataUInt64(elm.field_name));
                    break;
                case "float32":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataFloat32(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataFloat32(elm.field_name));
                    break;
                case "float64":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataFloat64(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataFloat64(elm.field_name));
                    break;
                case "bool":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + src.GetDataBool(elm.field_name));
                    tmp_bytes = BitConverter.GetBytes(src.GetDataBool(elm.field_name));
                    break;
                case "string":
                    //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " = " + System.Text.Encoding.ASCII.GetBytes(src.GetDataString(elm.field_name)));
                    tmp_bytes = System.Text.Encoding.ASCII.GetBytes(src.GetDataString(elm.field_name));
                    break;
                default:
                    throw new InvalidCastException("Error: Can not found ptype: " + elm.type_name);
            }
            var woff = off + elm.offset;
            //SimpleLogger.Get().Log(Level.INFO, elm.field_name + " : " + woff);
            //SimpleLogger.Get().Log(Level.INFO, "dst.len=" + dst_buffer.Length);
            //SimpleLogger.Get().Log(Level.INFO, "src.len=" + tmp_bytes.Length);
            Buffer.BlockCopy(tmp_bytes, 0, dst_buffer, woff, tmp_bytes.Length);
        }

        public IPduCommData ConvertToIoData(IPduWriter src)
        {
            return ConvertToIoData(src.GetReadOps());
        }
    }
}
