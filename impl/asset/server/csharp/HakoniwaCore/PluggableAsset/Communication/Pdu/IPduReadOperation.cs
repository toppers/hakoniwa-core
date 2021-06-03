using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduReadOperation
    {
        Int64 GetHeaderData(string field_name);
        SByte GetDataInt8(string field_name);
        Byte GetDataUInt8(string field_name);
        Int16 GetDataInt16(string field_name);
        UInt16 GetDataUInt16(string field_name);
        Int32 GetDataInt32(string field_name);
        UInt32 GetDataUInt32(string field_name);
        UInt64 GetDataUInt64(string field_name);
        Int64 GetDataInt64(string field_name);
        double GetDataDouble(string field_name);
        byte[] GetDataBytes(string field_name);
    }
}
