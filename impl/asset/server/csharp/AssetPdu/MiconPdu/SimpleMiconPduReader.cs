using Google.Protobuf;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Micon
{
    public class SimpleMiconPduReader : IPduReader
    {
        private string name;
        private Pdu pdu;
        private IPduReaderConverter converter = null;
        private bool is_set = false;
        public SimpleMiconPduReader(Pdu arg_pdu, string name)
        {
            this.pdu = arg_pdu;
            this.name = name;
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

        public string GetIoKey()
        {
            return this.name;
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

        public bool IsValidData()
        {
            return is_set;
        }

        public void Set(IPduCommData data)
        {
            PduCommBinaryData binary = null;
            if (data == null)
            {
                return;
            }

            if (data is PduCommBinaryData)
            {
                binary = (PduCommBinaryData)data;
            }
            if (binary == null)
            {
                throw new ArgumentException("Invalid data type:" + data.GetType());
            }
            if (this.converter == null)
            {
                throw new ArgumentException("Ev3PduReader converter null!!");
            }
            else
            {
                this.is_set = true;
                this.converter.ConvertToPduData(binary, this);
            }
        }

        public void SetConverter(IPduReaderConverter cnv)
        {
            this.converter = cnv;
        }
        public void Reset()
        {
            this.pdu.Reset();
        }
    }
}
