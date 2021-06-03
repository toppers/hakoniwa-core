using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Method.ROS
{
    interface IRosTopicIo
    {
        void Publish(IPduCommTypedData data);
        IPduCommTypedData Recv(string topic_name);
    }
}
