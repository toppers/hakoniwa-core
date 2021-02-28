using HakoniwaGrpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core
{
    public class Request
    {
        private AssetNotificationEvent ev;

        public Request(AssetNotificationEvent ev_type)
        {
            this.ev = ev_type;
        }

        public AssetNotificationEvent GetEvent()
        {
            return this.ev;
        }
    }
}
