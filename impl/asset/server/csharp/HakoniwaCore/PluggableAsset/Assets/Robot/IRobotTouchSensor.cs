using System.Collections;
using System.Collections.Generic;

namespace Hakoniwa.PluggableAsset.Assets.Robot
{
    public interface IRobotTouchSensor: IRobotSensor
    {
        bool IsPressed();
    }
}
