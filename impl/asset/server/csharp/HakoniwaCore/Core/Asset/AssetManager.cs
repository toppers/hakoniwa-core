using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset;
using Hakoniwa.PluggableAsset.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Asset
{
    public class AssetManager
    {
        private List<RegisteredAsset> asset_list;

        public List<RegisteredAsset> RefList()
        {
            var list = new List<RegisteredAsset>();
            foreach (var e in this.asset_list)
            {
                list.Add(e);
            }
            return list;
        }
        public List<IOutsideAssetController> RefOutsideAssetList()
        {
            var list = new List<IOutsideAssetController>();
            foreach (var e in this.asset_list)
            {
                if (e.GetController() is IOutsideAssetController)
                {
                    var asset = e.GetController() as IOutsideAssetController;
                    list.Add(asset);
                }
            }
            return list;
        }
        public List<IInsideAssetController> RefInsideAssetList()
        {
            var list = new List<IInsideAssetController>();
            foreach (var e in this.asset_list)
            {
                if (e.GetController() is IInsideAssetController)
                {
                    var asset = e.GetController() as IInsideAssetController;
                    list.Add(asset);
                }
            }
            return list;
        }

        public int GetAssetCount()
        {
            return asset_list.Count;
        }

        public AssetManager()
        {
            asset_list = new List<RegisteredAsset>();
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
        public void UpdateTime(string name)
        {
            foreach (var asset in asset_list)
            {
                if (asset.GetName().Equals(name))
                {
                    asset.UpdateTime();
                }
            }
        }
        public bool IsTimeout(string name, long timeout)
        {
            foreach (var asset in asset_list)
            {
                if (asset.GetName().Equals(name))
                {
                    return Utils.UtilTime.IsTimeout(asset.GetUpdateTime(), timeout);
                }
            }
            return true;
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
        public bool RegisterOutsideAsset(string name)
        {
            SimpleLogger.Get().Log(Level.INFO, "RegisterOutsideAsset :"  + name);
            if (IsExist(name))
            {
                SimpleLogger.Get().Log(Level.ERROR, "RegisterOutsideAsset already exist:" + name);
                return false;
            }
            var controller = AssetConfigLoader.GetOutsideAsset(name);
            if (controller == null)
            {
                SimpleLogger.Get().Log(Level.ERROR, "RegisterOutsideAsset not found controller:" + name);
                return false;
            }

            var asset = new RegisteredAsset(name, AssetType.Outside);
            asset.SetController(controller);
            asset_list.Add(asset);
            controller.Initialize();
            SimpleLogger.Get().Log(Level.INFO, "RegisterOutsideAsset Success:" + name);
            return true;
        }
        public bool RegisterInsideAsset(string name)
        {
            if (IsExist(name))
            {
                //Debug.Log("ERROR:already exit:" + name);
                return false;
            }
            var controller = AssetConfigLoader.GetInsideAsset(name);
            if (controller == null)
            {
                //Debug.Log("ERROR:Not found:" + name);
                return false;
            }
            var asset = new RegisteredAsset(name, AssetType.Inside);
            asset.SetController(controller);
            asset_list.Add(asset);
            SimpleLogger.Get().Log(Level.INFO, "RegisterInsideAsset Success:" + name);
            return true;
        }
        public void Unregister(string name)
        {
            RegisteredAsset entry = null;
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
                SimpleLogger.Get().Log(Level.INFO, "Unregister Success:" + name);
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
        public List<AssetEntry> GetAssetList(AssetType type)
        {
            List<AssetEntry> list = new List<AssetEntry>();
            foreach (var asset in asset_list)
            {
                if (asset.GetAssetType() == type)
                {
                    list.Add(new AssetEntry(asset.GetName()));
                }
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
