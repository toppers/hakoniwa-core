using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.Rpc
{
    class RpcConfig : IIoReaderConfig, IIoWriterConfig
    {
        public string asset_name;
        private string method_type;
        public int channel_id;
        private int pdu_size;
        public int write_count;
        public int PduSize
        {
            set { this.pdu_size = value; }
            get { return this.pdu_size; }
        }
        public string MethodType
        {
            set { this.method_type = value; }
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
