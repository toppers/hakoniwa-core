using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Robot.TB3
{
    public interface ITB3Parts
    {
        string GetMotor(int index);
        string GetIMU();
        string GetLaserScan();
    }
}
