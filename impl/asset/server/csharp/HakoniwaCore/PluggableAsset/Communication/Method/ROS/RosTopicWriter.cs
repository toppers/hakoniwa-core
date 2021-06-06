using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.ROS
{
    class RosTopicWriter : IIoWriter
    {
        RosTopicConfig ros_config = null;
        private IRosTopicIo io;
        
        public string Name { get; internal set; }

        public RosTopicWriter(RosTopicConfig config)
        {
            this.ros_config = config;
        }

        public string GetName()
        {
            return this.Name;
        }
        public void Flush(IPduCommData data)
        {
            IPduCommTypedData topic_data = data as IPduCommTypedData;
            this.io.Publish(topic_data);
        }

        public void Initialize(IIoWriterConfig config)
        {
            this.ros_config = config as RosTopicConfig;
            this.io = ros_config.ros_topic_io;
        }
    }
}
