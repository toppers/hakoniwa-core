using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.ROS
{
    class RosTopicConfig : IIoReaderConfig, IIoWriterConfig
    {
        public string name;
        public IRosTopicIo ros_topic_io;

        public RosTopicConfig(string method_name, IRosTopicIo io)
        {
            this.name = method_name;
            this.ros_topic_io = io;
        }

    }
}
