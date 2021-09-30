using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduWriteOperation
    {
        void SetData(string field_name, Byte value);
        void SetData(string field_name, SByte value);
        void SetData(string field_name, UInt16 value);
        void SetData(string field_name, Int16 value);
        void SetData(string field_name, UInt32 value);
        void SetData(string field_name, Int32 value);
        void SetData(string field_name, UInt64 value);
        void SetData(string field_name, Int64 value);
        void SetData(string field_name, double value);
        void SetData(string field_name, float value);
        void SetData(string field_name, string value);
        void SetData(string field_name, bool value);

        void SetData(string field_name, Byte[] value);
        void SetData(string field_name, SByte[] value);
        void SetData(string field_name, UInt16[] value);
        void SetData(string field_name, Int16[] value);
        void SetData(string field_name, UInt32[] value);
        void SetData(string field_name, Int32[] value);
        void SetData(string field_name, UInt64[] value);
        void SetData(string field_name, Int64[] value);
        void SetData(string field_name, double[] value);
        void SetData(string field_name, float[] value);
        void SetData(string field_name, string[] value);
        void SetData(string field_name, bool[] value);

        void SetData(string field_name, Pdu pdu);
        void SetData(string field_name, Pdu[] pdu);
        void InitializePduArray(string field_name, int array_size);

        Pdu Ref(string field_name);
        Pdu[] Refs(string field_name);
    }
}
