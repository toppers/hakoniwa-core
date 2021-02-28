using System;
using System.Collections.Generic;
using System.Text;

namespace HakoniwaCore
{
    public class AssetManager
    {
        private List<Asset> asset_list;

        public int GetAssetCount()
        {
            return asset_list.Count;
        }

        public AssetManager()
        {
            asset_list = new List<Asset>();
        }
        public bool IsExist(string name)
        {
            foreach (var asset in asset_list)
            {
                if (asset.GetName().Equals(name))
                {
                    return true;
                }
            }
            return false;
        }
        public AssetEvent GetEvent(string name)
        {
            foreach (var asset in asset_list)
            {
                if (asset.GetName().Equals(name))
                {
                    return asset.GetEvent();
                }
            }
            return null;
        }
        public void SetEvent(string name, AssetEvent aev)
        {
            foreach (var asset in asset_list)
            {
                if (asset.GetName().Equals(name))
                {
                    asset.SetEvent(aev);
                }
            }
            return;
        }
        /*
         * 箱庭アセットを登録する
         * 箱庭アセットを登録解除する
         */
        public bool Register(string name)
        {
            if (IsExist(name))
            {
                return false;
            }
            asset_list.Add(new Asset(name));
            return true;
        }
        public void Unregister(string name)
        {
            Asset entry = null;
            foreach (var asset in asset_list)
            {
                if (asset.GetName().Equals(name))
                {
                    entry = asset;
                    break;
                }
            }
            if (entry != null)
            {
                asset_list.Remove(entry);
            }
            return;
        }

        public List<AssetEntry> GetAssetList()
        {
            List<AssetEntry> list = new List<AssetEntry>();
            foreach (var asset in asset_list)
            {
                list.Add(new AssetEntry(asset.GetName()));
            }
            return list;
        }
    }

    public class AssetEntry
    {
        private string asset_name;

        public AssetEntry(string name)
        {
            this.asset_name = name;
        }
        public string GetName()
        {
            return this.asset_name;
        }
    }
}
