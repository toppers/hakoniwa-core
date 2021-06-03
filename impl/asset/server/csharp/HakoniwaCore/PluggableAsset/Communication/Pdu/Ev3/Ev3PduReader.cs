using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Ev3
{
    class Ev3PduReader : IPduReader
    {
        private PduConfig pdu_config;
        private string packet_header = "ETTX";
        private int packet_version = 0x1;
        private int packet_ext_off = 512;
        private int packet_ext_size = 512;
        private string name;
        private Pdu pdu;
        private IPduReaderConverter converter = null;

        public Ev3PduReader(string name)
        {
            this.name = name;
            this.pdu_config = new PduConfig(32);
            this.pdu_config.SetHeaderOffset("header", 0, 4);
            this.pdu_config.SetHeaderOffset("version", 4, 4);
            this.pdu_config.SetHeaderOffset("simulation_time", 8, 8);
            this.pdu_config.SetHeaderOffset("ext_off", 24, 4);
            this.pdu_config.SetHeaderOffset("ext_size", 28, 4);

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
            this.pdu = new Pdu(this.pdu_config, 1024);
        }

        public bool IsValidData()
        {
            if (this.pdu.GetBuffer() == null)
            {
                return false;
            }
            byte[] buf = this.pdu.GetBuffer();
            return IsValidPacket(ref buf);
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

        public string GetName()
        {
            return name;
        }

        public IPduReadOperation GetReadOps()
        {
            return this.pdu.GetPduReadOps();
        }
        public IPduWriteOperation GetWriteOps()
        {
            return this.pdu.GetPduWriteOps();
        }
        public void SetConverter(IPduReaderConverter cnv)
        {
            this.converter = cnv;
        }

        public void Set(IPduCommData data)
        {
            PduCommBinaryData binary = null;
            if (data == null)
            {
                return;
            }

            if (data.GetType() == typeof(PduCommBinaryData))
            {
                binary = (PduCommBinaryData)data;
            }
            if (binary == null)
            {
                throw new ArgumentException("Invalid data type:" + data.GetType());
            }
            if (this.converter == null)
            {
                this.pdu.SetBuffer(binary.GetData());
            }
            else
            {
                this.converter.ConvertToPduData(binary, this);
            }
        }
        public IPduCommData Get()
        {
            if (this.converter == null)
            {
                return new PduCommBinaryData(this.pdu.GetBuffer());
            }
            else
            {
                return this.converter.ConvertToIoData(this);
            }
        }

        public string GetIoKey()
        {
            return this.name;
        }
    }
}
