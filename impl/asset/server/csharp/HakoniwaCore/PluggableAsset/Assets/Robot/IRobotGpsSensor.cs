using System.Collections;
using System.Collections.Generic;

namespace Hakoniwa.PluggableAsset.Assets.Robot
{
    public interface IRobotGpsSensor : IRobotSensor
    {
        double GeLatitude();
        double GetLongitude();
        int GetStatus();
    }
}
