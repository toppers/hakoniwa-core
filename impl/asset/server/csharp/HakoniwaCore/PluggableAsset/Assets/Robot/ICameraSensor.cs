﻿using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Robot
{
    public interface ICameraSensor : IRobotSensor
    {
        void UpdateSensorData(Pdu pdu);
    }
}
