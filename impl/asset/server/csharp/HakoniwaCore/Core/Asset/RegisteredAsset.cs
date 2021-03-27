using Hakoniwa.PluggableAsset.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Asset
{
    public enum AssetType
    {
        Inside = 0,
        Outside = 1,
    }

    public class RegisteredAsset
    {
        static private int asset_id = 0;
        private int id;
        private string name;
        private AssetEvent asset_event = null;
        private AssetType type;
        private IAssetController controller;
        private long update_time;

        public RegisteredAsset(string asset_name, AssetType type)
        {
            this.id = RegisteredAsset.asset_id++;
            this.name = asset_name;
            this.type = type;
            this.update_time = Utils.UtilTime.GetUnixTime();
        }
        public void UpdateTime()
        {
            this.update_time = Utils.UtilTime.GetUnixTime();
        }
        public long GetUpdateTime()
        {
            return this.update_time;
        }
        public void SetController(IAssetController ctrl)
        {
            this.controller = ctrl;
        }
        public IAssetController GetController()
        {
            return this.controller;
        }
        public AssetType GetAssetType()
        {
            return this.type;
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
