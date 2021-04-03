using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduReader
    {
        string GetName();
        bool IsValidData();
        long GetHeaderData(string field_name);
        void GetData(string field_name, out int value);
        void GetData(string field_name, out ulong value);
        void GetData(string field_name, out double value);
        void Recv(IIoReader reader);

        void Send(IIoWriter writer);

    }
}
