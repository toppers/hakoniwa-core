using Google.Protobuf;
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
        private ObsoletePdu pdu;
        private IPduWriterConverter converter = null;

        public Ev3PduWriter(string name)
        {
            this.name = name;
            this.pdu_config = new PduConfig(32);
            this.pdu_config.SetHeaderOffset("hakoniwa_time", 16, 8);
            this.pdu_config.SetOffset("button", 0, 1);
            this.pdu_config.SetOffset("sensor_color0", 8, 4);
            this.pdu_config.SetOffset("sensor_reflect0", 12, 4);
            this.pdu_config.SetOffset("sensor_rgb_r0", 16, 4);
            this.pdu_config.SetOffset("sensor_rgb_g0", 20, 4);
            this.pdu_config.SetOffset("sensor_rgb_b0", 24, 4);

            this.pdu_config.SetOffset("sensor_color1", 132, 4);
            this.pdu_config.SetOffset("sensor_reflect1", 136, 4);
            this.pdu_config.SetOffset("sensor_rgb_r1", 140, 4);
            this.pdu_config.SetOffset("sensor_rgb_g1", 144, 4);
            this.pdu_config.SetOffset("sensor_rgb_b1", 148, 4);

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

            this.pdu = new ObsoletePdu(this.pdu_config, 1024);
            this.buffer = pdu.GetBuffer();
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
        }

        public IPduCommData Get()
        {
            if (this.converter == null)
            {
                return new PduCommBinaryData(this.buffer);
            }
            else
            {
                return this.converter.ConvertToIoData(this);
            }
        }

        public string GetName()
        {
            return this.name;
        }

        public IPduWriteOperation GetWriteOps()
        {
            return this.pdu.GetPduWriteOps();
        }

        public IPduReadOperation GetReadOps()
        {
            return this.pdu.GetPduReadOps();
        }

        public void SetConverter(IPduWriterConverter cnv)
        {
            this.converter = cnv;
        }
    }
}
