using Hakoniwa.Core.Asset;
using Hakoniwa.PluggableAsset.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation
{
    public interface ISimulationAssetManager
    {
        bool RegisterInsideAsset(string name);
        void Unregister(string name);
        List<IInsideAssetController> RefInsideAssetList();
    }
}
