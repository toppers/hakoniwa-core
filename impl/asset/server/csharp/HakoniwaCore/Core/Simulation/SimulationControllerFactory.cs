using Hakoniwa.PluggableAsset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation
{
    public class SimulationControllerFactory
    {
        public static ISimulationController Get(string asset_name)
        {
            if (asset_name == null)
            {
                return SimulationController.Get();
            }
            else if (AssetConfigLoader.core_config.cpp_mode.Equals("asset_rpc"))
            {
#if NO_USE_GRPC
                throw new NotSupportedException("ERROR: asset_rpc is not supported..");
#else
                return HakoRpcAssetSimulationController.Get(asset_name);
#endif
            }
            else
            {
                return HakoAssetSimulationController.Get(asset_name);
            }
        }
    }
}
