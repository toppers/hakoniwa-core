using Hakoniwa.Core.Utils.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Robot.TB3
{
    public class TB3PartsLoader : ITB3Parts
    {
        private TB3RobotPartsConfig config;
        private string config_path = "./tb3_parts.json";

        public void Load()
        {
            if (this.config != null)
            {
                return;
            }
            try
            {
                string jsonString = File.ReadAllText(config_path);
                config = JsonConvert.DeserializeObject<TB3RobotPartsConfig>(jsonString);
                SimpleLogger.Get().Log(Level.INFO, "jsonstring=" + jsonString);
            }
            catch (Exception)
            {
            }
        }
        public string GetCamera(out int update_scale)
        {
            update_scale = 1;
            if (config == null)
            {
                return null;
            }
            if (config.camera == null)
            {
                return null;
            }
            update_scale = config.camera.update_cycle;
            return config.camera.path;
        }

        public string GetIMU(out int update_scale)
        {
            update_scale = 1;
            if (config == null)
            {
                return null;
            }
            if (config.imu == null)
            {
                return null;
            }
            update_scale = config.imu.update_cycle;
            return config.imu.path;
        }

        public string GetLaserScan(out int update_scale)
        {
            update_scale = 1;
            if (config == null)
            {
                return null;
            }
            if (config.scan == null)
            {
                return null;
            }
            update_scale = config.scan.update_cycle;
            return config.scan.path;
        }

        public string GetMotor(int index, out int update_scale)
        {
            update_scale = 1;
            if (config == null)
            {
                return null;
            }
            if (config.motors == null)
            {
                return null;
            }
            if (index >= config.motors.Length)
            {
                return null;
            }
            update_scale = config.motors[index].update_cycle;
            return config.motors[index].path;
        }
    }
}
