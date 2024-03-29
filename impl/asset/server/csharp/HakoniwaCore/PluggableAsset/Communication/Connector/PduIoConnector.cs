﻿using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Connector
{
    public class PduIoConnector
    {
        public static List<PduIoConnector> connectors = new List<PduIoConnector>();
        public static PduIoConnector Create(string name)
        {
            var entry = new PduIoConnector(name);
            connectors.Add(entry);
            return entry;

        }
        public static void Reset()
        {
            foreach (var entry in connectors)
            {
                entry.ResetPdu();
            }

        }
        public static PduIoConnector Get(string name)
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
        private List<IPduWriter> pdu_writer = new List<IPduWriter>();
        private List<IPduReader> pdu_reader = new List<IPduReader>();

        public string GetName()
        {
            return name;
        }
        public bool AddWriter(IPduWriter writer)
        {
            foreach (var e in this.pdu_writer)
            {
                if (e.GetName().Equals(name))
                {
                    return false;
                }
            }
            SimpleLogger.Get().Log(Level.DEBUG, "PduIoConnector AddWriter: " + writer.GetName());
            this.pdu_writer.Add(writer);
            return true;
        }
        public bool AddReader(IPduReader reader)
        {
            foreach (var e in this.pdu_reader)
            {
                if (e.GetName().Equals(name))
                {
                    return false;
                }
            }
            this.pdu_reader.Add(reader);
            return true;
        }
        public IPduWriter GetWriter(string name)
        {
            foreach(var e in this.pdu_writer)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        public IPduReader GetReader(string name)
        {
            foreach (var e in this.pdu_reader)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        public void ResetPdu()
        {
            foreach (var e in this.pdu_reader)
            {
                e.Reset();
            }
            foreach (var e in this.pdu_writer)
            {
                e.Reset();
            }
        }

        private PduIoConnector(string name)
        {
            this.name = name;
        }

    }
}
