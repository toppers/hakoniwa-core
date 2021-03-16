using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Method
{
    public interface IIoWriter
    {
        void Initialize(IIoWriterConfig config);
        void Flush(ref byte[] buffer);
    }
}
