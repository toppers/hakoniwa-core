using HakoniwaGrpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace HakoniwaCore
{
    public class AssetEvent
    {
        private AssetNotificationEvent ev;
        public AssetEvent(AssetNotificationEvent ev_type)
        {
            this.ev = ev_type;
        }
        public AssetNotificationEvent GetEvent()
        {
            return this.ev;
        }
    }
}
