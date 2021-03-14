using Hakoniwa.Core.Communication.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Channel
{
    class ReaderChannel
    {
        private IIoReader reader;

        public ReaderChannel(IIoReader obj)
        {
            this.reader = obj;
        }

        public IIoReader GetReaer()
        {
            return this.reader;
        }

    }
}
