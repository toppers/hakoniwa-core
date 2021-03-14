using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Method
{
    public interface IIoReader
    {
        void Initialize(IIoReaderConfig config);

        void Recv(ref byte[] buf);
    }
}
