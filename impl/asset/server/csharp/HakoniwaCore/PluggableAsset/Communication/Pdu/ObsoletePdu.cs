using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public class ObsoletePdu
    {
        private PduConfig pdu_config;
        private byte[] buffer;
        private IPduReadOperation rdops;
        private IPduWriteOperation wrops;
        private string name = null;

        public void SetName(string arg_name)
        {
            this.name = arg_name;
        }
        public string GetName()
        {
            return this.name;
        }

        public ObsoletePdu(PduConfig config, int size)
        {
            buffer = new byte[size];
            this.pdu_config = config;
            this.rdops = new PduReadOperationImpl(this);
            this.wrops = new PduWriteOperationImpl(this);
        }
        public void SetBuffer(byte[] buf)
        {
            this.buffer = buf;
        }
        public PduConfig GetConfig()
        {
            return this.pdu_config;
        }
        public byte[] GetBuffer()
        {
            return this.buffer;
        }
        public IPduReadOperation GetPduReadOps()
        {
            return rdops;
        }
        public IPduWriteOperation GetPduWriteOps()
        {
            return wrops;
        }

    }
}
