using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.ROS
{
    class RosTopicReader : IIoReader
    {
        RosTopicConfig ros_config = null;
        private IRosTopicIo io;
        public string Name { get; internal set; }

        public string GetName()
        {
            return this.Name;
        }

        public void Initialize(IIoReaderConfig config)
        {
            this.ros_config = config as RosTopicConfig;
            this.io = ros_config.ros_topic_io;
        }

        public IPduCommData Recv(string io_key)
        {
            return this.io.Recv(io_key);
        }
    }
}
