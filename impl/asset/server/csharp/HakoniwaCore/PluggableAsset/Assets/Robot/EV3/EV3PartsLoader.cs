using Hakoniwa.Core.Utils.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Robot.EV3
{
    public class EV3PartsLoader : IEV3Parts
    {
        private EV3RobotPartsConfig config;
        private string config_path = "./ev3_parts.json";
        public void Load()
        {
            if (this.config != null)
            {
                return;
            }
            try
            {
                string jsonString = File.ReadAllText(config_path);
                config = JsonConvert.DeserializeObject<EV3RobotPartsConfig>(jsonString);
                SimpleLogger.Get().Log(Level.INFO, "jsonstring=" + jsonString);
            }
            catch (Exception)
            {
            }
        }

        public string getButtonSensor(ButtonSensorType type)
        {
            if (config == null)
            {
                return null;
            }
            switch (type)
            {
                case ButtonSensorType.BUTTON_SENSOR_LEFT:
                    return config.button_sensor.left;
                case ButtonSensorType.BUTTON_SENSOR_RIGHT:
                    return config.button_sensor.right;
                case ButtonSensorType.BUTTON_SENSOR_ENTER:
                    return config.button_sensor.enter;
                case ButtonSensorType.BUTTON_SENSOR_DOWN:
                    return config.button_sensor.down;
                case ButtonSensorType.BUTTON_SENSOR_UP:
                    return config.button_sensor.up;
                case ButtonSensorType.BUTTON_SENSOR_BACK:
                    return config.button_sensor.back;
            }
            throw new System.NotImplementedException();
        }

        public string GetColorSensor0()
        {
            if (config == null)
            {
                return null;
            }
            if (config.color_sensors == null)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no color_sensors");
                throw new InvalidOperationException();
            }
            return config.color_sensors[0];
        }

        public string GetColorSensor1()
        {
            if (config == null)
            {
                return null;
            }
            if (config.color_sensors == null)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no color_sensors");
                throw new InvalidOperationException();
            }
            if (config.color_sensors.Length < 2)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no color_sensors[1]");
                throw new InvalidOperationException();
            }
            return config.color_sensors[1];
        }

        public string getGpsSensor()
        {
            if (config == null)
            {
                return null;
            }
            return config.gps_sensor;
        }

        public string getGyroSensor()
        {
            if (config == null)
            {
                return null;
            }
            return config.gyro_sensor;
        }

        public string GetLed()
        {
            if (config == null)
            {
                return null;
            }
            return config.led;
        }

        public string GetMotorA()
        {
            if (config == null)
            {
                return null;
            }
            if (config.motors == null)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no motoros");
                throw new InvalidOperationException();
            }
            if (config.motors.Length < 1)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no motors[0]");
                throw new InvalidOperationException();
            }
            return config.motors[0];
        }

        public string GetMotorB()
        {
            if (config == null)
            {
                return null;
            }
            if (config.motors == null)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no motoros");
                throw new InvalidOperationException();
            }
            if (config.motors.Length < 2)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no motors[1]");
                throw new InvalidOperationException();
            }
            return config.motors[1];
        }

        public string GetMotorC()
        {
            if (config == null)
            {
                return null;
            }
            if (config.motors == null)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no motoros");
                throw new InvalidOperationException();
            }
            if (config.motors.Length < 3)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no motors[2]");
                throw new InvalidOperationException();
            }
            return config.motors[2];
        }

        public string getTouchSensor0()
        {
            if (config == null)
            {
                return null;
            }
            if (config.touch_sensors == null)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no touch_sensors");
                throw new InvalidOperationException();
            }
            if (config.touch_sensors.Length < 1)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no touch_sensors[0]");
                throw new InvalidOperationException();
            }
            return config.touch_sensors[0];
        }

        public string getTouchSensor1()
        {
            if (config == null)
            {
                return null;
            }
            if (config.touch_sensors == null)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no touch_sensors");
                throw new InvalidOperationException();
            }
            if (config.touch_sensors.Length < 2)
            {
                SimpleLogger.Get().Log(Level.ERROR, "EV3Parts config is not valid: no touch_sensors[1]");
                throw new InvalidOperationException();
            }
            return config.touch_sensors[1];
        }

        public string getUltraSonicSensor()
        {
            if (config == null)
            {
                return null;
            }
            return config.ultrasonic_sensor;
        }
    }
}
