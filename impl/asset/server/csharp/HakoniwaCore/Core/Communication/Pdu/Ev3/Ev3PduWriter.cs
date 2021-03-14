using Hakoniwa.Core.Communication.Channel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Pdu.Ev3
{
    class Ev3PduWriter : IPduWriter
    {
        private WriterChannel channel;
        private PduConfig pdu_config;
        private byte[] buffer;

        public Ev3PduWriter(WriterChannel ch)
        {
            this.channel = ch;
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
            this.channel.GetWriter().SetBuffer(ref this.buffer);

            this.pdu_config = new PduConfig(32);
            this.pdu_config.SetOffset("sensor_color", 8, 4);
            this.pdu_config.SetOffset("sensor_reflect", 12, 4);
            this.pdu_config.SetOffset("sensor_rgb_r", 16, 4);
            this.pdu_config.SetOffset("sensor_rgb_g", 20, 4);
            this.pdu_config.SetOffset("sensor_rgb_b", 24, 4);
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
        public void Send()
        {
            this.channel.GetWriter().Flush();
        }

        public void SetData(string field_name, int value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        public void SetData(string field_name, ulong value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        public void setData(string field_name, double value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }
    }
}
