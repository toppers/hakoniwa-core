using Hakoniwa.Core.Communication.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Pdu
{
    interface IPduWriter
    {
        void SetData(string field_name, int value);
        void SetData(string field_name, ulong value);
        void setData(string field_name, double value);
        void Send(IIoWriter writer);
    }
}
