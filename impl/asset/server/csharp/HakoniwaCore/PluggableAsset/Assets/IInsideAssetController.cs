using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets
{
    public interface IInsideAssetController : IAssetController
    {
        void DoUpdate();
    }
}
