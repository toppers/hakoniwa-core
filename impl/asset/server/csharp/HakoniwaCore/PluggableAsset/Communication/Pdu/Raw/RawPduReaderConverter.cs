using System;
using Hakoniwa.Core.Utils.Logger;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Raw
{
    class RawPduReaderConverter : IPduReaderConverter
    {
        public IPduCommData ConvertToIoData(IPduReader src)
        {
            if (!src.IsValidData())
            {
                return null;
            }
            return RawPduWriterConverter.ConvertToIoData(src.GetReadOps());
        }

        public void ConvertToPduData(IPduCommData src, IPduReader dst)
        {
            PduCommBinaryData src_data = src as PduCommBinaryData;
            byte[] buffer = src_data.GetData();
            string type_name = dst.GetWriteOps().Ref(null).GetName();
            var off_info = PduOffset.Get(type_name);
            if (off_info == null)
            {
                throw new InvalidOperationException("Error: Can not found offset: type=" + type_name);
            }
            ConvertFromStruct(off_info, 0, buffer, dst.GetWriteOps());
        }

        private static void ConvertFromStruct(PduBinOffsetInfo off_info, int base_off, byte[] src_buffer, IPduWriteOperation dst)
        {
            //SimpleLogger.Get().Log(Level.INFO, "TO PDU:Start Convert: package=" + off_info.package_name + " type=" + off_info.type_name);
            foreach (var elm in off_info.elms)
            {
                if (elm.is_primitive)
                {
                    //primitive
                    if (elm.is_array)
                    {
                        ConvertFromPrimtiveArray(elm, base_off, src_buffer, dst);
                    }
                    else
                    {
                        ConvertFromPrimtive(elm, base_off, src_buffer, dst);
                    }
                }
                else
                {
                    //struct
                    if (elm.is_array)
                    {
                        ConvertFromStructArray(elm, base_off + elm.offset, src_buffer, dst);
                    }
                    else
                    {
                        PduBinOffsetInfo struct_off_info = PduOffset.Get(elm.type_name);
                        ConvertFromStruct(struct_off_info, base_off + elm.offset, src_buffer, dst.Ref(elm.field_name).GetPduWriteOps());
                    }
                }
            }
        }


        private static void ConvertFromStructArray(PduBinOffsetElmInfo elm, int base_off, byte[] src_buffer, IPduWriteOperation dst)
        {
            PduBinOffsetInfo struct_off_info = PduOffset.Get(elm.type_name);
            for (int i = 0; i < elm.array_size; i++)
            {
                Pdu dst_data = dst.Refs(elm.field_name)[i];
                ConvertFromStruct(struct_off_info, base_off + (i * elm.elm_size), src_buffer, dst_data.GetPduWriteOps());
            }
        }

        private static void ConvertFromPrimtive(PduBinOffsetElmInfo elm, int base_off, byte[] src_buffer, IPduWriteOperation dst)
        {
            var off = base_off + elm.offset;
            switch (elm.type_name)
            {
                case "int8":
                    dst.SetData(elm.field_name, (sbyte)src_buffer[off]);
                    break;
                case "int16":
                    dst.SetData(elm.field_name, BitConverter.ToInt16(src_buffer, off));
                    break;
                case "int32":
                    dst.SetData(elm.field_name, BitConverter.ToInt32(src_buffer, off));
                    break;
                case "int64":
                    dst.SetData(elm.field_name, BitConverter.ToInt64(src_buffer, off));
                    break;
                case "uint8":
                    dst.SetData(elm.field_name, (byte)src_buffer[off]);
                    break;
                case "uint16":
                    dst.SetData(elm.field_name, BitConverter.ToUInt16(src_buffer, off));
                    break;
                case "uint32":
                    dst.SetData(elm.field_name, BitConverter.ToUInt32(src_buffer, off));
                    break;
                case "uint64":
                    dst.SetData(elm.field_name, BitConverter.ToUInt64(src_buffer, off));
                    break;
                case "float32":
                    dst.SetData(elm.field_name, BitConverter.ToSingle(src_buffer, off));
                    break;
                case "float64":
                    dst.SetData(elm.field_name, BitConverter.ToDouble(src_buffer, off));
                    break;
                case "bool":
                    dst.SetData(elm.field_name, BitConverter.ToBoolean(src_buffer, off));
                    break;
                case "string":
                    var bytes = new byte[elm.elm_size];
                    Buffer.BlockCopy(src_buffer, off, bytes, 0, bytes.Length);
                    dst.SetData(elm.field_name,
                        System.Text.Encoding.ASCII.GetString(bytes));
                    break;
                default:
                    throw new InvalidCastException("Error: Can not found ptype: " + elm.type_name);
            }
        }
        private static void ConvertFromPrimtiveArray(PduBinOffsetElmInfo elm, int base_off, byte[] src_buffer, IPduWriteOperation dst)
        {
            int roff = base_off + elm.offset;
            for (int i = 0; i < elm.array_size; i++)
            {
                //SimpleLogger.Get().Log(Level.INFO, "field=" + elm.field_name);
                //SimpleLogger.Get().Log(Level.INFO, "type=" + elm.type_name);
                var off = (roff + i * elm.elm_size);
                switch (elm.type_name)
                {
                    case "int8":
                        dst.SetData(elm.field_name, i, (sbyte)src_buffer[off]);
                        break;
                    case "int16":
                        dst.SetData(elm.field_name, i, BitConverter.ToInt16(src_buffer, off));
                        break;
                    case "int32":
                        dst.SetData(elm.field_name, i, BitConverter.ToInt32(src_buffer, off));
                        break;
                    case "int64":
                        dst.SetData(elm.field_name, i, BitConverter.ToInt64(src_buffer, off));
                        break;
                    case "uint8":
                        dst.SetData(elm.field_name, i, (byte)src_buffer[off]);
                        break;
                    case "uint16":
                        dst.SetData(elm.field_name, i, BitConverter.ToUInt16(src_buffer, off));
                        break;
                    case "uint32":
                        dst.SetData(elm.field_name, i, BitConverter.ToUInt32(src_buffer, off));
                        break;
                    case "uint64":
                        dst.SetData(elm.field_name, i, BitConverter.ToUInt64(src_buffer, off));
                        break;
                    case "float32":
                        dst.SetData(elm.field_name, i, BitConverter.ToSingle(src_buffer, off));
                        break;
                    case "float64":
                        dst.SetData(elm.field_name, i, BitConverter.ToDouble(src_buffer, off));
                        break;
                    case "bool":
                        dst.SetData(elm.field_name, i, BitConverter.ToBoolean(src_buffer, off));
                        break;
                    case "string":
                        var bytes = new byte[elm.elm_size];
                        Buffer.BlockCopy(src_buffer, off, bytes, 0, bytes.Length);
                        dst.SetData(elm.field_name, i,
                            System.Text.Encoding.ASCII.GetString(bytes));
                        break;
                    default:
                        throw new InvalidCastException("Error: Can not found ptype: " + elm.type_name);
                }
            }
        }

    }
}
