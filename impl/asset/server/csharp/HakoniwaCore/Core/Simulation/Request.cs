using Hakoniwa.Core.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation
{
    public class Request
    {
        private CoreAssetNotificationEvent ev;

        public Request(CoreAssetNotificationEvent ev_type)
        {
            this.ev = ev_type;
        }

        public CoreAssetNotificationEvent GetEvent()
        {
            return this.ev;
        }
    }
}
