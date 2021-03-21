using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Connector
{
    class PduChannelConnector
    {
        public static List<PduChannelConnector> connectors = new List<PduChannelConnector>();
        public static PduChannelConnector Create(string name)
        {
            var entry = new PduChannelConnector(name);
            connectors.Add(entry);
            return entry;

        }
        public static PduChannelConnector Get(string name)
        {
            foreach (var entry in connectors)
            {
                if (entry.name.Equals(name))
                {
                    return entry;
                }
            }
            return null;
        }
        private string name;
        private WriterConnector writer_connector;
        private ReaderConnector reader_connector;

        public string GetName()
        {
            return name;
        }
        public WriterConnector Writer
        {
            set { this.writer_connector = value; }
            get { return this.writer_connector;  }
        }
        public ReaderConnector Reader
        {
            set { this.reader_connector = value; }
            get { return this.reader_connector; }
        }

        private PduChannelConnector(string name)
        {
            this.name = name;
        }

    }
}
