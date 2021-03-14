using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Pdu
{
    interface IPduReader
    {
        bool IsValidData();
        void GetData(string field_name, out int value);
        void GetData(string field_name, out ulong value);
        void GetData(string field_name, out double value);
        void Recv();

    }
}
