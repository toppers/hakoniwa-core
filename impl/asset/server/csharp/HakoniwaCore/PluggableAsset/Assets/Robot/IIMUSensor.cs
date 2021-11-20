using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Robot
{
    public interface IIMUSensor :  IRobotSensor
    {
        void UpdateSensorData(Pdu pdu);
        IRobotVector3 GetDeltaEulerAngle();
        IRobotVector3 GetPosition();
    }
}
