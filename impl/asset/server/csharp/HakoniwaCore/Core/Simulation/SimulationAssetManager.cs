using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation
{
    class SimulationAssetManager : ISimulationAssetManager
    {
        private System.Object lockObj = new System.Object();
        private AssetManager asset_mgr = new AssetManager();

        public List<IInsideAssetController> RefInsideAssetList()
        {
            lock (this.lockObj)
            {
                return this.asset_mgr.RefInsideAssetList();
            }
        }

        public bool RegisterInsideAsset(string name)
        {
            lock (this.lockObj)
            {
                return this.asset_mgr.RegisterInsideAsset(name);
            }
        }

        public void Unregister(string name)
        {
            lock (this.lockObj)
            {
                try
                {
                    this.asset_mgr.Unregister(name);
                }
                catch (Exception e)
                {
                    SimpleLogger.Get().Log(Level.ERROR, "Unregister error:" + name);
                    SimpleLogger.Get().Log(Level.TRACE, e);
                }
            }
        }
    }
}
