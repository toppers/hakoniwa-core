﻿
namespace Hakoniwa.PluggableAsset.Assets.Robot.EV3
{
    public interface IEV3Parts
    {
        string GetMotorA();
        string GetMotorB();
        string GetMotorC();
        string GetLed();
        string GetColorSensor();
        string getUltraSonicSensor();
        string getTouchSensor0();
        string getTouchSensor1();
        string getGyroSensor();
        string getGpsSensor();
    }
}
