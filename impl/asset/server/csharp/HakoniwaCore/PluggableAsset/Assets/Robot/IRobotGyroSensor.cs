using System.Collections;
using System.Collections.Generic;

namespace Hakoniwa.PluggableAsset.Assets.Robot
{
    public interface IRobotGyroSensor : IRobotSensor
    {
        float GetDegree();
        float GetDegreeRate();
        void ClearDegree();
    }
}
