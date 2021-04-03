﻿using Google.Protobuf;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Ev3
{
    class Ev3PduWriter : IPduWriter
    {
        private PduConfig pdu_config;
        private byte[] buffer;
        private string name;

        public Ev3PduWriter(string name)
        {
            this.name = name;
            this.buffer = new byte[1024];
            {
                //header
                byte[] byte_array = new byte[4];
                byte_array[0] = 0x45; /* E */
                byte_array[1] = 0x54; /* T */
                byte_array[2] = 0x52; /* R */
                byte_array[3] = 0x58; /* X */
                Buffer.BlockCopy(byte_array, 0, buffer, 0, byte_array.Length);
                int[] tmp_buf = new int[7];
                tmp_buf[0] = 1; //version
                tmp_buf[1] = 0; //reserve
                tmp_buf[2] = 0; //reserve
                tmp_buf[3] = 0; //unity time
                tmp_buf[4] = 0; //unity time
                tmp_buf[5] = 512; //ext_off
                tmp_buf[6] = 512; //ext_size
                Buffer.BlockCopy(tmp_buf, 0, buffer, 4, 28);
            }

            this.pdu_config = new PduConfig(32);
            this.pdu_config.SetHeaderOffset("hakoniwa_time", 16, 8);
            this.pdu_config.SetOffset("sensor_color0", 8, 4);
            this.pdu_config.SetOffset("sensor_reflect0", 12, 4);
            this.pdu_config.SetOffset("sensor_rgb_r0", 16, 4);
            this.pdu_config.SetOffset("sensor_rgb_g0", 20, 4);
            this.pdu_config.SetOffset("sensor_rgb_b0", 24, 4);
            this.pdu_config.SetOffset("sensor_gyroscope", 28, 16);//TODO
            this.pdu_config.SetOffset("gyro_degree", 28, 4);
            this.pdu_config.SetOffset("gyro_degree_rate", 32, 4);
            this.pdu_config.SetOffset("sensor_ultrasonic", 88, 4);
            this.pdu_config.SetOffset("touch_sensor0", 112, 4);
            this.pdu_config.SetOffset("touch_sensor1", 124, 4);
            this.pdu_config.SetOffset("motor_angle_a", 256, 4);
            this.pdu_config.SetOffset("motor_angle_b", 260, 4);
            this.pdu_config.SetOffset("motor_angle_c", 264, 4);
            this.pdu_config.SetOffset("gps_lat", 480, 8);
            this.pdu_config.SetOffset("gps_lon", 488, 8);

        }
        public void Send(IIoWriter writer)
        {
            writer.Flush(ref this.buffer);
        }

        public void SetData(string field_name, int value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }
        private UInt32 GetDataUInt32(string field_name)
        {
            return BitConverter.ToUInt32(this.buffer, pdu_config.GetOffset(field_name));
        }
        private Int32 GetDataInt32(string field_name)
        {
            return BitConverter.ToInt32(this.buffer, pdu_config.GetOffset(field_name));
        }
        private ulong GetDataUint64(string field_name)
        {
            return BitConverter.ToUInt64(this.buffer, pdu_config.GetOffset(field_name));
        }
        private double GetDataDouble(string field_name)
        {
            return BitConverter.ToDouble(this.buffer, pdu_config.GetOffset(field_name));
        }
        private double GetDataFloat64(string field_name)
        {
            return BitConverter.ToDouble(this.buffer, pdu_config.GetOffset(field_name));
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
        public string GetName()
        {
            return name;
        }

        public void SetHeaderData(string field_name, long value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }
        private ulong GetHeaderData(string field_name)
        {
            return BitConverter.ToUInt64(this.buffer, pdu_config.GetHeaderOffset(field_name));
        }
#if false
        public void SendProtoBuffer(IIoWriter writer)
        {
            byte[] protbuf = this.ProtoBufConvert();
            writer.Flush(ref protbuf);
        }
        private byte[] ProtoBufConvert()
        {
            var protbuf = new Ev3PduSensor
            {
                Header = new Ev3PduSensor.Types.Header
                {
                    Name = "ETRX",
                    HakoniwaTime = this.GetHeaderData("hakoniwa_time"),
                    ExtOff = 512,
                    ExtSize = 512
                },
                Body = new Ev3PduSensor.Types.Body
                {
                    ColorSensors = 
                    {
                        new Ev3PduSensor.Types.Body.Types.ColorSensor {
                            Color = this.GetDataUInt32("sensor_color0"),
                            Reflect = this.GetDataUInt32("sensor_reflect0"),
                            RgbR = this.GetDataUInt32("sensor_rgb_r0"),
                            RgbG = this.GetDataUInt32("sensor_rgb_g0"),
                            RgbB = this.GetDataUInt32("sensor_rgb_b0"),
                        },
                    },
                    TouchSensors =
                    {
                        new Ev3PduSensor.Types.Body.Types.TouchSensor {
                            Value = this.GetDataUInt32("touch_sensor0"),
                        },
                        new Ev3PduSensor.Types.Body.Types.TouchSensor {
                            Value = this.GetDataUInt32("touch_sensor1"),
                        },
                    },
                    SensorUltrasonic = this.GetDataUInt32("sensor_ultrasonic"),
                    GyroDegree = this.GetDataInt32("gyro_degree"),
                    GyroDegreeRate = this.GetDataInt32("gyro_degree_rate"),
                    MotorAngleA = this.GetDataUInt32("motor_angle_a"),
                    MotorAngleB = this.GetDataUInt32("motor_angle_b"),
                    MotorAngleC = this.GetDataUInt32("motor_angle_c"),
                    GpsLat = this.GetDataDouble("gps_lat"),
                    GpsLon = this.GetDataDouble("gps_lon"),
                },
            };
            var stream = new MemoryStream();
            protbuf.WriteTo(stream);
            return stream.ToArray();
        }
#endif
    }
}
