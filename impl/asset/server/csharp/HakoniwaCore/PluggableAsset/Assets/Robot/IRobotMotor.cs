using System.Collections;
using System.Collections.Generic;

namespace Hakoniwa.PluggableAsset.Assets.Robot
{
    public interface IRobotMotor : IRobotActuator
    {
        void SetForce(int force);
        void SetStop(bool stop);
        void SetTargetVelicty(int targetVelocity);
    }
}
