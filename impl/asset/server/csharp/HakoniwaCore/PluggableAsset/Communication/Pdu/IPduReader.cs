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
        void SetConverter(IPduReaderConverter cnv);
        void Set(IPduCommData data);
        string GetIoKey();
        IPduCommData Get();
        IPduReadOperation GetReadOps();
        IPduWriteOperation GetWriteOps();
    }
}
