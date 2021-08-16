using Google.Protobuf;
using Hakoniwa.PluggableAsset.Communication.Method;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Micon
{
    public class SimpleMiconPduWriter : IPduWriter
    {
        private string name;
        private Pdu pdu;
        private IPduWriterConverter converter = null;

        public SimpleMiconPduWriter(Pdu arg_pdu, string name)
        {
            this.name = name;
            this.pdu = arg_pdu;
        }
        public IPduCommData Get()
        {
            if (this.converter == null)
            {
                throw new ArgumentException("Converter is not set");
            }
            else
            {
                return this.converter.ConvertToIoData(this);
            }
        }

        public string GetName()
        {
            return this.name;
        }

        public IPduReadOperation GetReadOps()
        {
            return this.pdu.GetPduReadOps();
        }

        public IPduWriteOperation GetWriteOps()
        {
            return this.pdu.GetPduWriteOps();
        }

        public void SetConverter(IPduWriterConverter cnv)
        {
            this.converter = cnv;
        }
        public void Reset()
        {
            this.pdu.Reset();
        }
    }
}
