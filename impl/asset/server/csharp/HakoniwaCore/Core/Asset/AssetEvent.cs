using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Asset
{
    public enum CoreAssetNotificationEvent
    {
        Start = 0,
        Stop = 1,
        Running = 2,
        None = 3,
    }

    public class AssetEvent
    {
        private CoreAssetNotificationEvent ev;
        public AssetEvent(CoreAssetNotificationEvent ev_type)
        {
            this.ev = ev_type;
        }
        public CoreAssetNotificationEvent GetEvent()
        {
            return this.ev;
        }
    }
}
