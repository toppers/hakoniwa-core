using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduWriterConverter
    {
        IPduCommData ConvertToIoData(IPduWriter src);
    }
}
