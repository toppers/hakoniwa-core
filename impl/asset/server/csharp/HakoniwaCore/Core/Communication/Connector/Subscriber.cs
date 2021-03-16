using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Connector
{
    class Subscriber
    {
        public static List<Subscriber> subscribers = new List<Subscriber>();
        public static Subscriber Create(string name)
        {
            var entry = new Subscriber(name);
            subscribers.Add(entry);
            return entry;

        }
        public static Subscriber Get(string name)
        {
            foreach (var entry in subscribers)
            {
                if (entry.name.Equals(name))
                {
                    return entry;
                }
            }
            return null;
        }
        private string name;
        private List<ReaderConnector> list = new List<ReaderConnector>();
        private Subscriber(string name)
        {
            this.name = name;
        }

        public List<ReaderConnector> GetConnectors()
        {
            return this.list;
        }
    }
}
