using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduWriter
    {
        string GetName();

        void SetHeaderData(string field_name, long value);
        void SetData(string field_name, int value);
        void SetData(string field_name, ulong value);
        void SetData(string field_name, double value);
        void Send(IIoWriter writer);
    }
}
