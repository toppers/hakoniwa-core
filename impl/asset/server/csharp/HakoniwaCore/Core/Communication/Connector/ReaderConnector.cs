﻿using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Communication.Channel;
using Hakoniwa.Core.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Connector
{
    class ReaderConnector
    {
        private static List<ReaderConnector> connectors = new List<ReaderConnector>();
        public static ReaderConnector Create(IPduReader p, ReaderChannel c)
        {
            var entry = new ReaderConnector(p, c);
            connectors.Add(entry);
            return entry;
        }
        private ReaderChannel src_channel;
        private IPduReader dst_pdu;
        private ReaderConnector(IPduReader p, ReaderChannel c)
        {
            this.dst_pdu = p;
            this.src_channel = c;
        }

        public void Send()
        {
            this.dst_pdu.Recv(this.src_channel.GetReaer());
        }

        public IPduReader GetPdu()
        {
            return this.dst_pdu;
        }
    }
}
