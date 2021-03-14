using Hakoniwa.Core.Communication.Channel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Pdu.Ev3
{
    class Ev3PduReader : IPduReader
    {
        private ReaderChannel channel;
        private PduConfig pdu_config;
        private byte[] buffer;
        private int io_size = 1024;
        private string packet_header = "ETTX";
        private int packet_version = 0x1;
        private int packet_ext_off = 512;
        private int packet_ext_size = 512;

        public Ev3PduReader(ReaderChannel ch)
        {
            this.channel = ch;
            this.buffer = new byte[io_size];
            this.pdu_config = new PduConfig(32);
            this.pdu_config.SetOffset("led", 0, 4);
            this.pdu_config.SetOffset("motor_power_a", 4, 4);
            this.pdu_config.SetOffset("motor_power_b", 8, 4);
            this.pdu_config.SetOffset("motor_power_c", 12, 4);
            this.pdu_config.SetOffset("motor_stop_a", 20, 4);
            this.pdu_config.SetOffset("motor_stop_b", 24, 4);
            this.pdu_config.SetOffset("motor_stop_c", 28, 4);
            this.pdu_config.SetOffset("motor_reset_angle_a", 36, 16);
            this.pdu_config.SetOffset("motor_reset_angle_b", 40, 4);
            this.pdu_config.SetOffset("motor_reset_angle_c", 44, 4);
            this.pdu_config.SetOffset("gyro_reset", 52, 4);
        }

        public void GetData(string field_name, out int value)
        {
            value = BitConverter.ToInt32(this.buffer, pdu_config.GetOffset(field_name));
        }

        public void GetData(string field_name, out ulong value)
        {
            value = BitConverter.ToUInt64(this.buffer, pdu_config.GetOffset(field_name));
        }

        public void GetData(string field_name, out double value)
        {
            value = BitConverter.ToDouble(this.buffer, pdu_config.GetOffset(field_name));
        }

        public bool IsValidData()
        {
            return IsValidPacket(ref this.buffer);
        }
        private bool IsValidPacket(ref byte[] data)
        {
            byte[] byte_array = new byte[4];
            Buffer.BlockCopy(data, 0, byte_array, 0, byte_array.Length);
            string recv_header = System.Text.Encoding.ASCII.GetString(byte_array);
            if (!recv_header.Equals(this.packet_header))
            {
                return false;
            }
            int[] tmp_buf = new int[7];
            Buffer.BlockCopy(data, 4, tmp_buf, 0, 28);
            if (tmp_buf[0] != this.packet_version)
            {
                return false; //version
            }
            if (tmp_buf[5] != this.packet_ext_off)
            {
                return false; //ext_off
            }
            if (tmp_buf[6] != this.packet_ext_size)
            {
                return false; //ext_size
            }
            return true;
        }
        public void Recv()
        {
            this.channel.GetReaer().Recv(ref this.buffer);
        }

    }
}
