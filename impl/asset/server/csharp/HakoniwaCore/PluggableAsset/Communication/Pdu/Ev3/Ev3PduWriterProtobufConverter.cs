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
                    HakoniwaTime = (ulong)src.GetReadOps().Ref("header").GetDataInt64("hakoniwa_time"),
                    Version = 0x1,
                    ExtOff = 512,
                    ExtSize = 512
                },
                Body = new Ev3PduSensor.Types.Body
                {
                    Buttons = ByteString.CopyFrom(src.GetReadOps().GetDataUInt8Array("buttons")),
                    ColorSensors =
                    {
                        new Ev3PduSensor.Types.Body.Types.ColorSensor {
                            Color = src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("color"),
                            Reflect = src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("reflect"),
                            RgbR = src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("rgb_r"),
                            RgbG = src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("rgb_g"),
                            RgbB = src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("rgb_b"),
                        },
                        new Ev3PduSensor.Types.Body.Types.ColorSensor {
                            Color = src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("color"),
                            Reflect = src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("reflect"),
                            RgbR = src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("rgb_r"),
                            RgbG = src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("rgb_g"),
                            RgbB = src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("rgb_b"),
                        },
                    },
                    TouchSensors =
                    {
                        new Ev3PduSensor.Types.Body.Types.TouchSensor {
                            Value = src.GetReadOps().Refs("touch_sensors")[0].GetDataUInt32("value"),
                        },
                        new Ev3PduSensor.Types.Body.Types.TouchSensor {
                            Value = src.GetReadOps().Refs("touch_sensors")[1].GetDataUInt32("value"),
                        },
                    },
                    SensorUltrasonic = src.GetReadOps().GetDataUInt32("sensor_ultrasonic"),
                    GyroDegree = src.GetReadOps().GetDataInt32("gyro_degree"),
                    GyroDegreeRate = src.GetReadOps().GetDataInt32("gyro_degree_rate"),
                    MotorAngleA = src.GetReadOps().GetDataUInt32Array("motor_angle")[0],
                    MotorAngleB = src.GetReadOps().GetDataUInt32Array("motor_angle")[1],
                    MotorAngleC = src.GetReadOps().GetDataUInt32Array("motor_angle")[2],
                    GpsLat = src.GetReadOps().GetDataFloat64("gps_lat"),
                    GpsLon = src.GetReadOps().GetDataFloat64("gps_lon"),
                },
            };
            var stream = new MemoryStream();
            protbuf.WriteTo(stream);
            var buf = stream.ToArray();
            return new PduCommBinaryData(buf);
        }
    }
}
