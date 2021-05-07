using Google.Protobuf;
using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Ev3
{
    class Ev3PduProtobufWriter : IPduWriter
    {
        private PduConfig pdu_config;
        private byte[] buffer;
        private string name;

        public Ev3PduProtobufWriter(string name)
        {
            this.name = name;
            this.buffer = new byte[1024];

            this.pdu_config = new PduConfig(32);
            this.pdu_config.SetHeaderOffset("hakoniwa_time", 16, 8);
            this.pdu_config.SetOffset("button",          0,  1);
            this.pdu_config.SetOffset("sensor_color0",   8,  4);
            this.pdu_config.SetOffset("sensor_reflect0", 12, 4);
            this.pdu_config.SetOffset("sensor_rgb_r0",   16, 4);
            this.pdu_config.SetOffset("sensor_rgb_g0",   20, 4);
            this.pdu_config.SetOffset("sensor_rgb_b0",   24, 4);

            this.pdu_config.SetOffset("sensor_color1", 132, 4);
            this.pdu_config.SetOffset("sensor_reflect1", 136, 4);
            this.pdu_config.SetOffset("sensor_rgb_r1", 140, 4);
            this.pdu_config.SetOffset("sensor_rgb_g1", 144, 4);
            this.pdu_config.SetOffset("sensor_rgb_b1", 148, 4);

            this.pdu_config.SetOffset("sensor_gyroscope",   28, 16);//TODO
            this.pdu_config.SetOffset("gyro_degree",        28, 4);
            this.pdu_config.SetOffset("gyro_degree_rate",   32, 4);
            this.pdu_config.SetOffset("sensor_ultrasonic",  88, 4);
            this.pdu_config.SetOffset("touch_sensor0",     112, 4);
            this.pdu_config.SetOffset("touch_sensor1",     124, 4);
            this.pdu_config.SetOffset("motor_angle_a",     256, 4);
            this.pdu_config.SetOffset("motor_angle_b",     260, 4);
            this.pdu_config.SetOffset("motor_angle_c",     264, 4);
            this.pdu_config.SetOffset("gps_lat",           480, 8);
            this.pdu_config.SetOffset("gps_lon",           488, 8);
        }
        public string GetName()
        {
            return this.name;
        }

        public void SetBuffer(byte[] buf)
        {
            this.buffer = buf;
        }
        public void Send(IIoWriter writer)
        {
            var pbuffer = ProtoBufConvert(this);
            writer.Flush(ref pbuffer);
        }

        public static byte[] ProtoBufConvert(IPduWriter obj)
        {
            var protbuf = new Ev3PduSensor
            {
                Header = new Ev3PduSensor.Types.Header
                {
                    Name = "ETRX",
                    HakoniwaTime = (ulong)obj.GetHeaderData("hakoniwa_time"),
                    Version = 0x1,
                    ExtOff = 512,
                    ExtSize = 512
                },
                Body = new Ev3PduSensor.Types.Body
                {
                    Buttons = ByteString.CopyFrom(obj.GetDataBytes("button")),
                    ColorSensors =
                    {
                        new Ev3PduSensor.Types.Body.Types.ColorSensor {
                            Color = obj.GetDataUInt32("sensor_color0"),
                            Reflect = obj.GetDataUInt32("sensor_reflect0"),
                            RgbR = obj.GetDataUInt32("sensor_rgb_r0"),
                            RgbG = obj.GetDataUInt32("sensor_rgb_g0"),
                            RgbB = obj.GetDataUInt32("sensor_rgb_b0"),
                        },
                        new Ev3PduSensor.Types.Body.Types.ColorSensor {
                            Color = obj.GetDataUInt32("sensor_color1"),
                            Reflect = obj.GetDataUInt32("sensor_reflect1"),
                            RgbR = obj.GetDataUInt32("sensor_rgb_r1"),
                            RgbG = obj.GetDataUInt32("sensor_rgb_g1"),
                            RgbB = obj.GetDataUInt32("sensor_rgb_b1"),
                        },
                    },
                    TouchSensors =
                    {
                        new Ev3PduSensor.Types.Body.Types.TouchSensor {
                            Value = obj.GetDataUInt32("touch_sensor0"),
                        },
                        new Ev3PduSensor.Types.Body.Types.TouchSensor {
                            Value = obj.GetDataUInt32("touch_sensor1"),
                        },
                    },
                    SensorUltrasonic = obj.GetDataUInt32("sensor_ultrasonic"),
                    GyroDegree = obj.GetDataInt32("gyro_degree"),
                    GyroDegreeRate = obj.GetDataInt32("gyro_degree_rate"),
                    MotorAngleA = obj.GetDataUInt32("motor_angle_a"),
                    MotorAngleB = obj.GetDataUInt32("motor_angle_b"),
                    MotorAngleC = obj.GetDataUInt32("motor_angle_c"),
                    GpsLat = obj.GetDataDouble("gps_lat"),
                    GpsLon = obj.GetDataDouble("gps_lon"),
                },
            };
            var stream = new MemoryStream();
            protbuf.WriteTo(stream);
            return stream.ToArray();
        }

        public byte[] GetDataBytes(string field_name)
        {
            if (field_name != null)
            {
                byte[] tmp_buf = new byte[this.pdu_config.GetSize(field_name)];
                Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
                return tmp_buf;
            }
            else
            {
                return null;
            }
        }

        public void SetData(string field_name, int value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        public void SetData(string field_name, ulong value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        public void SetData(string field_name, double value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        public void SetHeaderData(string field_name, long value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }
        public UInt32 GetDataUInt32(string field_name)
        {
            return BitConverter.ToUInt32(this.buffer, pdu_config.GetOffset(field_name));
        }
        public Int32 GetDataInt32(string field_name)
        {
            return BitConverter.ToInt32(this.buffer, pdu_config.GetOffset(field_name));
        }
        private ulong GetDataUint64(string field_name)
        {
            return BitConverter.ToUInt64(this.buffer, pdu_config.GetOffset(field_name));
        }
        public double GetDataDouble(string field_name)
        {
            return BitConverter.ToDouble(this.buffer, pdu_config.GetOffset(field_name));
        }
        private double GetDataFloat64(string field_name)
        {
            return BitConverter.ToDouble(this.buffer, pdu_config.GetOffset(field_name));
        }
        public long GetHeaderData(string field_name)
        {
            return BitConverter.ToInt64(this.buffer, pdu_config.GetHeaderOffset(field_name));
        }

    }
}
