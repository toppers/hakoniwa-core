using Hakoniwa.PluggableAsset.Assets;
using Hakoniwa.PluggableAsset.Communication.Channel;
using Hakoniwa.PluggableAsset.Communication.Pdu;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Connector
{
    public class WriterConnector
    {
        public string Name { get; set; }
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
#if false
        public void SendProtoBuffer()
        {
            this.src_pdu.SendProtoBuffer(this.dst_channel.GetWriter());
        }
#endif
        public IPduWriter GetPdu()
        {
            return this.src_pdu;
        }
    }
}
