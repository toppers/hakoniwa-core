
namespace Hakoniwa.PluggableAsset.Assets.Robot.EV3
{
    public enum ButtonSensorType
    {
        BUTTON_SENSOR_LEFT = 0,
        BUTTON_SENSOR_RIGHT = 1,
        BUTTON_SENSOR_UP = 2,
        BUTTON_SENSOR_DOWN = 3,
        BUTTON_SENSOR_ENTER = 4,
        BUTTON_SENSOR_BACK = 5,
    }
    public interface IEV3Parts
    {
        void Load();
        string GetMotorA();
        string GetMotorB();
        string GetMotorC();
        string GetLed();
        string GetColorSensor0();
        string GetColorSensor1();
        string getUltraSonicSensor();
        string getTouchSensor0();
        string getTouchSensor1();
        string getButtonSensor(ButtonSensorType type);
        string getGyroSensor();
        string getGpsSensor();
    }
    [System.Serializable]
    public class EV3RobotButtonPartsConfig
    {
        public string left;
        public string right;
        public string up;
        public string down;
        public string enter;
        public string back;
    }

    [System.Serializable]
    public class EV3RobotPartsConfig
    {
        public string[] color_sensors;
        public string[] touch_sensors;
        public string ultrasonic_sensor;
        public EV3RobotButtonPartsConfig button_sensor;
        public string gps_sensor;
        public string gyro_sensor;
        public string led;
        public string[] motors;
    }

}
