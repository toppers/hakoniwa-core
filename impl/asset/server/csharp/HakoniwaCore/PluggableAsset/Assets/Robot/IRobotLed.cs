using System.Collections;
using System.Collections.Generic;

namespace Hakoniwa.PluggableAsset.Assets.Robot
{
    public enum LedColor
    {
        LED_COLOR_NONE = 0,
        LED_COLOR_RED = 1,
        LED_COLOR_GREEN = 2,
        LED_COLOR_ORANGE = 3,
    }

    public interface IRobotLed : IRobotActuator
    {
        void SetLedColor(LedColor color);
    }
}

