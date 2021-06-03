using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.ROS
{
    class RosTopicConfig : IIoReaderConfig, IIoWriterConfig
    {
        public string name;
        public string message_type_name;
        public IRosTopicIo ros_topic_io;

        public RosTopicConfig(string topic_name, string type_name, IRosTopicIo io)
        {
            this.name = topic_name;
            this.message_type_name = type_name;
            this.ros_topic_io = io;
        }

    }
}
