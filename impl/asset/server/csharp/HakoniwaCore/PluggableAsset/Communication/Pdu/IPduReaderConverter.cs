using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduReaderConverter
    {
        void ConvertToPduData(IPduCommData src, IPduReader dst);
        IPduCommData ConvertToIoData(IPduReader src);
    }
}
