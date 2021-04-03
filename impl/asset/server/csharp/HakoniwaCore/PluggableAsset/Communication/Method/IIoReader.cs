using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method
{
    public interface IIoReader
    {
        void Initialize(IIoReaderConfig config);

        string GetName();
        byte[] Recv();
    }
}
