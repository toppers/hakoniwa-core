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

        private static int ConvertFromStruct(PduBinOffsetInfo off_info, int off, byte[] src_buffer, IPduWriteOperation dst)
        {
            int next_off = off;
            SimpleLogger.Get().Log(Level.INFO, "TO PDU:Start Convert: package=" + off_info.package_name + " type=" + off_info.type_name);
            foreach (var elm in off_info.elms)
            {
                if (elm.is_primitive)
                {
                    //primitive
                    if (elm.is_array)
                    {
                        next_off = ConvertFromPrimtiveArray(elm, next_off, src_buffer, dst);
                    }
                    else
                    {
                        next_off = ConvertFromPrimtive(elm, next_off, src_buffer, dst);
                    }
                }
                else
                {
                    //struct
                    if (elm.is_array)
                    {
                        next_off = ConvertFromStructArray(elm, next_off, src_buffer, dst);
                    }
                    else
                    {
                        PduBinOffsetInfo struct_off_info = PduOffset.Get(elm.type_name);
                        next_off = ConvertFromStruct(struct_off_info, next_off, src_buffer, dst.Ref(elm.field_name).GetPduWriteOps());
                    }
                }
            }
            return next_off;
        }


        private static int ConvertFromStructArray(PduBinOffsetElmInfo elm, int off, byte[] src_buffer, IPduWriteOperation dst)
        {
            int next_off = off;
            PduBinOffsetInfo struct_off_info = PduOffset.Get(elm.type_name);
            for (int i = 0; i < elm.array_size; i++)
            {
                Pdu dst_data = dst.Refs(elm.field_name)[i];
                next_off = ConvertFromStruct(struct_off_info, next_off, src_buffer, dst_data.GetPduWriteOps());
            }
            return next_off;
        }

        private static int ConvertFromPrimtive(PduBinOffsetElmInfo elm, int off, byte[] src_buffer, IPduWriteOperation dst)
        {
            switch (elm.type_name)
            {
                case "int8":
                    dst.SetData(elm.field_name, BitConverter.ToChar(src_buffer, off));
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
                    dst.SetData(elm.field_name, BitConverter.ToChar(src_buffer, off));
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
            return (off + elm.elm_size);
        }
        private static int ConvertFromPrimtiveArray(PduBinOffsetElmInfo elm, int off, byte[] src_buffer, IPduWriteOperation dst)
        {
            int next_off = off;
            for (int i = 0; i < elm.array_size; i++)
            {
                switch (elm.type_name)
                {
                    case "int8":
                        dst.SetData(elm.field_name, i, BitConverter.ToChar(src_buffer, next_off));
                        break;
                    case "int16":
                        dst.SetData(elm.field_name, i, BitConverter.ToInt16(src_buffer, next_off));
                        break;
                    case "int32":
                        dst.SetData(elm.field_name, i, BitConverter.ToInt32(src_buffer, next_off));
                        break;
                    case "int64":
                        dst.SetData(elm.field_name, i, BitConverter.ToInt64(src_buffer, next_off));
                        break;
                    case "uint8":
                        dst.SetData(elm.field_name, i, BitConverter.ToChar(src_buffer, next_off));
                        break;
                    case "uint16":
                        dst.SetData(elm.field_name, i, BitConverter.ToUInt16(src_buffer, next_off));
                        break;
                    case "uint32":
                        dst.SetData(elm.field_name, i, BitConverter.ToUInt32(src_buffer, next_off));
                        break;
                    case "uint64":
                        dst.SetData(elm.field_name, i, BitConverter.ToUInt64(src_buffer, next_off));
                        break;
                    case "float32":
                        dst.SetData(elm.field_name, i, BitConverter.ToSingle(src_buffer, next_off));
                        break;
                    case "float64":
                        dst.SetData(elm.field_name, i, BitConverter.ToDouble(src_buffer, next_off));
                        break;
                    case "bool":
                        dst.SetData(elm.field_name, i, BitConverter.ToBoolean(src_buffer, next_off));
                        break;
                    case "string":
                        var bytes = new byte[elm.elm_size];
                        Buffer.BlockCopy(src_buffer, next_off, bytes, 0, bytes.Length);
                        dst.SetData(elm.field_name, i,
                            System.Text.Encoding.ASCII.GetString(bytes));
                        break;
                    default:
                        throw new InvalidCastException("Error: Can not found ptype: " + elm.type_name);
                }
                next_off += elm.elm_size;
            }
            return next_off;
        }

    }
}
