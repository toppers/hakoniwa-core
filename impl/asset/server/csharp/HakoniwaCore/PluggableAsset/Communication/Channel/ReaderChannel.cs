using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Channel
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
