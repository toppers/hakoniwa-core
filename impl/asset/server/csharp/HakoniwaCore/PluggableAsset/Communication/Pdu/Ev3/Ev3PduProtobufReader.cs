﻿using Google.Protobuf;
using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Ev3
{
    class Ev3PduProtobufReader : IPduReader
    {
        private PduConfig pdu_config;
        private byte[] buffer;
        private byte[] protbuf;
        private Ev3PduActuator packet;
        private string name;
        private int io_size = 1024;
        private string packet_header = "ETTX";
        private int packet_version = 0x1;
        private int packet_ext_off = 512;
        private int packet_ext_size = 512;

        public Ev3PduProtobufReader(string name)
        {
            this.name = name;
            this.buffer = new byte[io_size];
            Buffer.SetByte(this.buffer, 0, 0);
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
        }

        public void GetData(string field_name, out Int32 value)
        {
            value = BitConverter.ToInt32(this.buffer, pdu_config.GetOffset(field_name));
        }
        public void GetData(string field_name, out UInt32 value)
        {
            value = BitConverter.ToUInt32(this.buffer, pdu_config.GetOffset(field_name));
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
        public void Recv(IIoReader reader)
        {
            this.protbuf = reader.Recv();
            if (this.protbuf == null)
            {
                return;
            }
            this.ProtoBufConvert(protbuf);
        }
        public void Send(IIoWriter writer)
        {
            if (this.protbuf == null)
            {
                return;
            }
            writer.Flush(ref this.protbuf);
        }
        public void SetProtobuf(byte [] buf)
        {
            this.protbuf = buf;
        }
        private void ProtoBufConvert(byte [] protobuf)
        {
            var parser = new MessageParser<Ev3PduActuator>(() => new Ev3PduActuator());
            this.packet = parser.ParseFrom(new MemoryStream(protobuf));
            this.SetHeaderData("header", this.packet.Header.Name);
            this.SetHeaderData("version", this.packet.Header.Version);
            this.SetHeaderData("simulation_time", (long)this.packet.Header.AssetTime);
            this.SetHeaderData("ext_off", this.packet.Header.ExtOff);
            this.SetHeaderData("ext_size", this.packet.Header.ExtSize);

            this.SetData("led", this.packet.Body.Leds.ToByteArray());
            this.SetData("motor_power_a", this.packet.Body.Motors[0].Power);
            this.SetData("motor_power_b", this.packet.Body.Motors[1].Power);
            this.SetData("motor_power_c", this.packet.Body.Motors[2].Power);

            this.SetData("motor_stop_a", this.packet.Body.Motors[0].Stop);
            this.SetData("motor_stop_b", this.packet.Body.Motors[1].Stop);
            this.SetData("motor_stop_c", this.packet.Body.Motors[2].Stop);

            this.SetData("motor_reset_angle_a", this.packet.Body.Motors[0].ResetAngle);
            this.SetData("motor_reset_angle_b", this.packet.Body.Motors[1].ResetAngle);
            this.SetData("motor_reset_angle_c", this.packet.Body.Motors[2].ResetAngle);

            this.SetData("gyro_reset", this.packet.Body.GyroReset);
        }
        public static byte[] ProtoBufConvert(IPduReader obj)
        {
            var protbuf = new Ev3PduActuator
            {
                Header = new Ev3PduActuator.Types.Header
                {
                    Name = "ETTX",
                    AssetTime = (ulong)obj.GetHeaderData("simulation_time"),
                    Version = 0x1,
                    ExtOff = 512,
                    ExtSize = 512
                },
                Body = new Ev3PduActuator.Types.Body
                {
                    Leds = ByteString.CopyFrom(obj.GetDataBytes("led")),
                    Motors =
                    {
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = obj.GetDataInt32("motor_power_a"),
                            Stop = obj.GetDataUInt32("motor_stop_a"),
                            ResetAngle = obj.GetDataUInt32("motor_reset_angle_a"),
                        },
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = obj.GetDataInt32("motor_power_b"),
                            Stop = obj.GetDataUInt32("motor_stop_b"),
                            ResetAngle = obj.GetDataUInt32("motor_reset_angle_b"),
                        },
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = obj.GetDataInt32("motor_power_c"),
                            Stop = obj.GetDataUInt32("motor_stop_c"),
                            ResetAngle = obj.GetDataUInt32("motor_reset_angle_c"),
                        },
                    },
                    GyroReset = obj.GetDataUInt32("gyro_reset"),
                },
            };
            var stream = new MemoryStream();
            protbuf.WriteTo(stream);
            return stream.ToArray();
        }
        public string GetName()
        {
            return name;
        }

        public long GetHeaderData(string field_name)
        {
            return BitConverter.ToInt64(this.buffer, pdu_config.GetHeaderOffset(field_name));
        }

        private void SetData(string field_name, byte[] value)
        {
            Buffer.BlockCopy(value, 0, this.buffer, pdu_config.GetOffset(field_name), value.Length);
        }

        private void SetData(string field_name, int value)
        {
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        private void SetData(string field_name, ulong value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        private void SetData(string field_name, double value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
        }

        private void SetHeaderData(string field_name, long value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }
        private void SetHeaderData(string field_name, int value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }
        private void SetHeaderData(string field_name, string value)
        {
            //Debug.Log("filed_name=" + field_name + " value=" + value);
            byte[] tmp_buf = System.Text.Encoding.ASCII.GetBytes(value);
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetHeaderOffset(field_name), tmp_buf.Length);
        }
        public byte[] GetDataBytes(string field_name)
        {
            byte[] tmp_buf = new byte[this.pdu_config.GetSize(field_name)];
            Buffer.BlockCopy(tmp_buf, 0, this.buffer, pdu_config.GetOffset(field_name), tmp_buf.Length);
            return tmp_buf;
        }

        public double GetDataDouble(string field_name)
        {
            double ret = 0;
            this.GetData(field_name, out ret);
            return ret;
        }

        public UInt32 GetDataUInt32(string field_name)
        {
            UInt32 ret;
            this.GetData(field_name, out ret);
            return ret;
        }

        public Int32 GetDataInt32(string field_name)
        {
            Int32 ret = 0;
            this.GetData(field_name, out ret);
            return ret;
        }
    }
}
