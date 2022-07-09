using Hakoniwa.Core.Utils.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Ev3
{
    class Ev3PduWriterRawConverter : IPduWriterConverter
    {
        private byte[] my_buffer = new byte[1024];

        public IPduCommData ConvertToIoData(IPduWriter src)
        {
            byte[] tmp_bytes = null;
            //Sensor
            //Ev3PduSensorHeader header
            {
                //string name
                var tmp_name = "ETRX";
                tmp_bytes = System.Text.Encoding.ASCII.GetBytes(tmp_name);
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 0, 4);

                //uint32 version
                uint tmp_value = 0x1;
                tmp_bytes = BitConverter.GetBytes(tmp_value);
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 4, tmp_bytes.Length);

                //int64 hakoniwa_time
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Ref("head").GetDataInt64("hakoniwa_time"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 16, tmp_bytes.Length);

                //uint32 ext_off
                tmp_value = 512;
                tmp_bytes = BitConverter.GetBytes(tmp_value);
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 24, tmp_bytes.Length);

                //uint32 ext_size
                tmp_value = 512;
                tmp_bytes = BitConverter.GetBytes(tmp_value);
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 28, tmp_bytes.Length);
            }
            //SimpleLogger.Get().Log(Level.DEBUG, "dbg1");
            //uint8[1] buttons
            uint button_value = src.GetReadOps().GetDataUInt8Array("buttons")[0];
            tmp_bytes = BitConverter.GetBytes(button_value);
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 0), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg2");
            //Ev3PduColorSensor[2] color_sensors
            {
                //uint32 color
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("color"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 8), tmp_bytes.Length);
                //uint32 reflect
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("reflect"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 12), tmp_bytes.Length);
                //uint32 rgb_r
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("rgb_r"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 16), tmp_bytes.Length);
                //uint32 rgb_g
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("rgb_g"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 20), tmp_bytes.Length);
                //uint32 rgb_b
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[0].GetDataUInt32("rgb_b"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 24), tmp_bytes.Length);

                //uint32 color
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("color"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 132), tmp_bytes.Length);
                //uint32 reflect
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("reflect"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 136), tmp_bytes.Length);
                //uint32 rgb_r
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("rgb_r"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 140), tmp_bytes.Length);
                //uint32 rgb_g
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("rgb_g"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 144), tmp_bytes.Length);
                //uint32 rgb_b
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("color_sensors")[1].GetDataUInt32("rgb_b"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 148), tmp_bytes.Length);

            }
            //SimpleLogger.Get().Log(Level.DEBUG, "dbg3");
            //Ev3PduTouchSensor[2] touch_sensors
            {
                //uint32 value
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("touch_sensors")[0].GetDataUInt32("value"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 112), tmp_bytes.Length);

                //uint32 value
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("touch_sensors")[1].GetDataUInt32("value"));
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 124), tmp_bytes.Length);
            }
            //SimpleLogger.Get().Log(Level.DEBUG, "dbg4");
            //uint32[3] motor_angle
            {
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataUInt32Array("motor_angle")[0]);
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 256), tmp_bytes.Length);
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataUInt32Array("motor_angle")[1]);
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 260), tmp_bytes.Length);
                tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataUInt32Array("motor_angle")[2]);
                Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 264), tmp_bytes.Length);
            }
            //SimpleLogger.Get().Log(Level.DEBUG, "dbg5");
            //int32 gyro_degree
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataInt32("gyro_degree"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 28), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg6");
            //int32 gyro_degree_rate
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataInt32("gyro_degree_rate"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 32), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg7");
            //uint32 sensor_ultrasonic
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataUInt32("sensor_ultrasonic"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 88), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg8");
            //float64 gps_lat
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataFloat64("gps_lat"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 480), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg9");
            //float64 gps_lon
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataFloat64("gps_lon"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 488), tmp_bytes.Length);
            //SimpleLogger.Get().Log(Level.DEBUG, "dbg10");

            var obj = new PduCommBinaryData(this.my_buffer);
            //SimpleLogger.Get().Log(Level.DEBUG, "dbg11");
            return obj;
        }
    }
}
