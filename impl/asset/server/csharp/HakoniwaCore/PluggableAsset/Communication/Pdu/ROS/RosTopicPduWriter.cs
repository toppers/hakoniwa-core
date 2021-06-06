using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.ROS
{
    public class RosTopicPduWriter : IPduWriter
    {
        private string topic_name;
        private string topic_msg_type;
        private Pdu pdu;
        private IPduWriterConverter converter = null;

        public RosTopicPduWriter(Pdu arg_pdu, string name, string msg_type)
        {
            this.pdu = arg_pdu;
            this.topic_name = name;
            this.topic_msg_type = msg_type;
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
            return this.topic_name;
        }
        public string GetTypeName()
        {
            return this.topic_msg_type;
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
    }
}
