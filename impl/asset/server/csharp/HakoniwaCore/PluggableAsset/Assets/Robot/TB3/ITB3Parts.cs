using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Robot.TB3
{
    public interface ITB3Parts
    {
        void Load();
        string GetMotor(int index, out int update_scale);
        string GetIMU(out int update_scale);
        string GetLaserScan(out int update_scale);
        string GetCamera(out int update_scale);
    }
    [System.Serializable]
    public class TB3RobotPartsEntryConfig
    {
        public string path;
        public int update_cycle;
    }
    [System.Serializable]
    public class TB3RobotPartsConfig
    {
        public TB3RobotPartsEntryConfig[] motors;
        public TB3RobotPartsEntryConfig imu;
        public TB3RobotPartsEntryConfig scan;
        public TB3RobotPartsEntryConfig camera;
    }
}
