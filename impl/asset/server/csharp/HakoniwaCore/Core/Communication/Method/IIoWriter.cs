using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Method
{
    public interface IIoWriter
    {
        void Initialize(IIoWriterConfig config);
        void SetBuffer(ref byte[] buffer);
        void Flush();
    }
}
