using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Communication.Channel;
using Hakoniwa.Core.Communication.Pdu;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Connector
{
    class WriterConnector
    {
        private static List<WriterConnector> connectors = new List<WriterConnector>();
        public static WriterConnector Create(IPduWriter p, WriterChannel c)
        {
            var entry = new WriterConnector(p, c);
            connectors.Add(entry);
            return entry;
        }
        private IPduWriter src_pdu;
        private WriterChannel dst_channel;

        private WriterConnector(IPduWriter p, WriterChannel c)
        {
            this.src_pdu = p;
            this.dst_channel = c;
        }

        public void Send()
        {
            this.src_pdu.Send(this.dst_channel.GetWriter());
        }

        public IPduWriter GetPdu()
        {
            return this.src_pdu;
        }
    }
}
