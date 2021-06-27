using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.ROS
{
    public class RosTopicPduReader : IPduReader
    {
        private string name;
        private string topic_name;
        private string topic_msg_type;
        private Pdu pdu;
        private IPduReaderConverter converter = null;
        private bool is_set = false;

        public RosTopicPduReader(Pdu arg_pdu, string name, string tname, string msg_type)
        {
            this.pdu = arg_pdu;
            this.name = name;
            this.topic_name = tname;
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

        public string GetIoKey()
        {
            return this.topic_name;
        }
        public string GetTopicName()
        {
            return this.topic_name;
        }
        public string GetTypeName()
        {
            return this.topic_msg_type;
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
            if (this.converter == null)
            {
                throw new ArgumentException("Converter is not set");
            }
            IPduCommTypedData typedData = null;
            if (data != null)
            {
                this.is_set = true;
            }
            else
            {
                return;
            }
            if (data is IPduCommTypedData)
            {
                typedData = (IPduCommTypedData)data;
            }
            if (typedData == null) 
            { 
                throw new ArgumentException("Invalid data type:" + data.GetType());
            }
            else
            {
                this.converter.ConvertToPduData(typedData, this);
            }
        }

        public void SetConverter(IPduReaderConverter cnv)
        {
            this.converter = cnv;
        }
    }
}
