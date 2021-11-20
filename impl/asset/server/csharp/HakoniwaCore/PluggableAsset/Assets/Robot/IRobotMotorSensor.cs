using System.Collections;
using System.Collections.Generic;


namespace Hakoniwa.PluggableAsset.Assets.Robot
{
    public interface IRobotMotorSensor : IRobotSensor
    {
        float GetDegree();
        void ClearDegree();
        float GetCurrentAngle();
        float GetCurrentAngleVelocity();
        float GetRadius();
    }
}
