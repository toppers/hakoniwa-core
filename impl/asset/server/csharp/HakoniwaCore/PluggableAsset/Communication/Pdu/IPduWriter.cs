using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduWriter
    {
        string GetName();
        void SetConverter(IPduWriterConverter cnv);
        IPduCommData Get();
        IPduWriteOperation GetWriteOps();
        IPduReadOperation GetReadOps();
    }
}
