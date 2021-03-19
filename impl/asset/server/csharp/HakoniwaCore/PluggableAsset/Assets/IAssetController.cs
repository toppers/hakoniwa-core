using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets
{
    public interface IAssetController
    {
        void Initialize();
        string GetName();
    }
}
