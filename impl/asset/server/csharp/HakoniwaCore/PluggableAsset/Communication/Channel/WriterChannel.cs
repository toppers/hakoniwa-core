using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Channel
{
    class WriterChannel
    {
        private IIoWriter writer;

        public WriterChannel(IIoWriter obj)
        {
            this.writer = obj;
        }

        public IIoWriter GetWriter()
        {
            return this.writer;
        }
    }
}
