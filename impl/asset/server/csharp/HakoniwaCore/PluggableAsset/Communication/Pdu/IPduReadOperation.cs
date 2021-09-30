using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduReadOperation
    {
        SByte GetDataInt8(string field_name);
        Byte GetDataUInt8(string field_name);
        Int16 GetDataInt16(string field_name);
        UInt16 GetDataUInt16(string field_name);
        Int32 GetDataInt32(string field_name);
        UInt32 GetDataUInt32(string field_name);
        UInt64 GetDataUInt64(string field_name);
        Int64 GetDataInt64(string field_name);
        float GetDataFloat32(string field_name);
        double GetDataFloat64(string field_name);
        string GetDataString(string field_name);
        bool GetDataBool(string field_name);

        SByte[] GetDataInt8Array(string field_name);
        Byte[] GetDataUInt8Array(string field_name);
        Int16[] GetDataInt16Array(string field_name);
        UInt16[] GetDataUInt16Array(string field_name);
        Int32[] GetDataInt32Array(string field_name);
        UInt32[] GetDataUInt32Array(string field_name);
        UInt64[] GetDataUInt64Array(string field_name);
        Int64[] GetDataInt64Array(string field_name);
        float[] GetDataFloat32Array(string field_name);
        double[] GetDataFloat64Array(string field_name);
        string[] GetDataStringArray(string field_name);
        bool[] GetDataBoolArray(string field_name);

        Pdu Ref(string field_name);
        Pdu[] Refs(string field_name);

    }
}
