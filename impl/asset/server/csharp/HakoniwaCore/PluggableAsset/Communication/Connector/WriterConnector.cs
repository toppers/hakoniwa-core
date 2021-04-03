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
        public static WriterConnector Create(IPduReader p, WriterChannel c)
        {
            var entry = new WriterConnector(p, c);
            connectors.Add(entry);
            return entry;
        }
        private IPduWriter w_pdu;
        private IPduReader r_pdu;
        private WriterChannel dst_channel;

        private WriterConnector(IPduWriter p, WriterChannel c)
        {
            this.w_pdu = p;
            this.dst_channel = c;
        }
        private WriterConnector(IPduReader p, WriterChannel c)
        {
            this.r_pdu = p;
            this.dst_channel = c;
        }

        public void SendWriterPdu()
        {
            if (this.w_pdu != null)
            {
                this.w_pdu.Send(this.dst_channel.GetWriter());
            }
        }
        public void SendReaderPdu()
        {
            if (this.r_pdu != null)
            {
                this.r_pdu.Send(this.dst_channel.GetWriter());
            }
        }
        public IPduWriter GetPdu()
        {
            return this.w_pdu;
        }
    }
}
