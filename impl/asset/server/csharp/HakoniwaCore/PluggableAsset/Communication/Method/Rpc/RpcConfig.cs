using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Rpc
{
    class RpcConfig : IIoReaderConfig, IIoWriterConfig
    {
        public string asset_name;
        public string method_type;
        public int channel_id;
        private int pdu_size;
        public int PduSize
        {
            set { this.pdu_size = value; }
            get { return this.pdu_size; }
        }
        public string get_method_type()
        {
            if (this.method_type == null)
            {
                return "UDP";
            }
            else
            {
                return this.method_type;
            }
        }
    }
}
