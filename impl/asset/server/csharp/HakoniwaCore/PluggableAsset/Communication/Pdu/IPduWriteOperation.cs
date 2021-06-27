using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduWriteOperation
    {
        void SetHeaderData(string field_name, string value);
        void SetHeaderData(string field_name, Int32 value);
        void SetHeaderData(string field_name, Int64 value);
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
        void SetData(string field_name, byte[] value);
    }
}
