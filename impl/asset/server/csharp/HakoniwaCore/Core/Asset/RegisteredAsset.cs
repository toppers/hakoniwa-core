using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Asset
{
    class RegisteredAsset
    {
        static private int asset_id = 0;
        private int id;
        private string name;
        private AssetEvent asset_event = null;

        public RegisteredAsset(string asset_name)
        {
            this.id = RegisteredAsset.asset_id++;
            this.name = asset_name;
        }

        public int GetId()
        {
            return id;
        }
        public string GetName()
        {
            return name;
        }

        public void SetEvent(AssetEvent aev)
        {
            this.asset_event = aev;
        }
        public AssetEvent GetEvent()
        {
            AssetEvent aev = this.asset_event;
            this.asset_event = null;
            return aev;
        }

    }
}
