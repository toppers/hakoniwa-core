using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public interface IHakoEnvObstacle : IHakoEnv
    {
        bool IsTouched();
    }
}

