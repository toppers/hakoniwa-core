using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Connector
{
    class Publisher
    {
        public static List<Publisher> publishers = new List<Publisher>();
        public static Publisher Create(string name)
        {
            var entry = new Publisher(name);
            publishers.Add(entry);
            return entry;

        }
        public static Publisher Get(string name)
        {
            foreach (var entry in publishers)
            {
                if (entry.name.Equals(name))
                {
                    return entry;
                }
            }
            return null;
        }
        private string name;
        private List<WriterConnector> list = new List<WriterConnector>();
        private Publisher(string name)
        {
            this.name = name;
        }

        public List<WriterConnector>  GetConnectors()
        {
            return this.list;
        }
    }
}
