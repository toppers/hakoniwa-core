using Google.Protobuf;
using Hakoniwa.Core.Utils.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Ev3
{
    class Ev3PduWriterProtobufConverter : IPduWriterConverter
    {
        public IPduCommData ConvertToIoData(IPduWriter src)
        {
            var protbuf = new Ev3PduSensor
            {
                Header = new Ev3PduSensor.Types.Header
                {
                    Name = "ETRX",
                    HakoniwaTime = (ulong)src.GetReadOps().GetHeaderData("hakoniwa_time"),
                    Version = 0x1,
                    ExtOff = 512,
                    ExtSize = 512
                },
                Body = new Ev3PduSensor.Types.Body
                {
                    Buttons = ByteString.CopyFrom(src.GetReadOps().GetDataBytes("button")),
                    ColorSensors =
                    {
                        new Ev3PduSensor.Types.Body.Types.ColorSensor {
                            Color = src.GetReadOps().GetDataUInt32("sensor_color0"),
                            Reflect = src.GetReadOps().GetDataUInt32("sensor_reflect0"),
                            RgbR = src.GetReadOps().GetDataUInt32("sensor_rgb_r0"),
                            RgbG = src.GetReadOps().GetDataUInt32("sensor_rgb_g0"),
                            RgbB = src.GetReadOps().GetDataUInt32("sensor_rgb_b0"),
                        },
                        new Ev3PduSensor.Types.Body.Types.ColorSensor {
                            Color = src.GetReadOps().GetDataUInt32("sensor_color1"),
                            Reflect = src.GetReadOps().GetDataUInt32("sensor_reflect1"),
                            RgbR = src.GetReadOps().GetDataUInt32("sensor_rgb_r1"),
                            RgbG = src.GetReadOps().GetDataUInt32("sensor_rgb_g1"),
                            RgbB = src.GetReadOps().GetDataUInt32("sensor_rgb_b1"),
                        },
                    },
                    TouchSensors =
                    {
                        new Ev3PduSensor.Types.Body.Types.TouchSensor {
                            Value = src.GetReadOps().GetDataUInt32("touch_sensor0"),
                        },
                        new Ev3PduSensor.Types.Body.Types.TouchSensor {
                            Value = src.GetReadOps().GetDataUInt32("touch_sensor1"),
                        },
                    },
                    SensorUltrasonic = src.GetReadOps().GetDataUInt32("sensor_ultrasonic"),
                    GyroDegree = src.GetReadOps().GetDataInt32("gyro_degree"),
                    GyroDegreeRate = src.GetReadOps().GetDataInt32("gyro_degree_rate"),
                    MotorAngleA = src.GetReadOps().GetDataUInt32("motor_angle_a"),
                    MotorAngleB = src.GetReadOps().GetDataUInt32("motor_angle_b"),
                    MotorAngleC = src.GetReadOps().GetDataUInt32("motor_angle_c"),
                    GpsLat = src.GetReadOps().GetDataFloat64("gps_lat"),
                    GpsLon = src.GetReadOps().GetDataFloat64("gps_lon"),
                },
            };
            //SimpleLogger.Get().Log(Level.INFO, "hakoniwa_time=" + protbuf.Header.HakoniwaTime);
            //SimpleLogger.Get().Log(Level.INFO, "Pdu:hakoniwa_time=" + src.GetReadOps().GetHeaderData("hakoniwa_time"));
            var stream = new MemoryStream();
            protbuf.WriteTo(stream);
            var buf = stream.ToArray();
            return new PduCommBinaryData(buf);
        }
    }
}
