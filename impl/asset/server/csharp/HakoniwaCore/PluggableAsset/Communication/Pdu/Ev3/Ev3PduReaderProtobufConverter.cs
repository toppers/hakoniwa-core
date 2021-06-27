using Google.Protobuf;
using Hakoniwa.Core.Utils.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Ev3
{
    class Ev3PduReaderProtobufConverter : IPduReaderConverter
    {
        public IPduCommData ConvertToIoData(IPduReader src)
        {
            var protbuf = new Ev3PduActuator
            {
                Header = new Ev3PduActuator.Types.Header
                {
                    Name = "ETTX",
                    AssetTime = (ulong)src.GetReadOps().GetHeaderData("simulation_time"),
                    Version = 0x1,
                    ExtOff = 512,
                    ExtSize = 512
                },
                Body = new Ev3PduActuator.Types.Body
                {
                    Leds = ByteString.CopyFrom(src.GetReadOps().GetDataBytes("led")),
                    Motors =
                    {
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = src.GetReadOps().GetDataInt32("motor_power_a"),
                            Stop = src.GetReadOps().GetDataUInt32("motor_stop_a"),
                            ResetAngle = src.GetReadOps().GetDataUInt32("motor_reset_angle_a"),
                        },
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = src.GetReadOps().GetDataInt32("motor_power_b"),
                            Stop = src.GetReadOps().GetDataUInt32("motor_stop_b"),
                            ResetAngle = src.GetReadOps().GetDataUInt32("motor_reset_angle_b"),
                        },
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = src.GetReadOps().GetDataInt32("motor_power_c"),
                            Stop = src.GetReadOps().GetDataUInt32("motor_stop_c"),
                            ResetAngle = src.GetReadOps().GetDataUInt32("motor_reset_angle_c"),
                        },
                    },
                    GyroReset = src.GetReadOps().GetDataUInt32("gyro_reset"),
                },
            };
            var stream = new MemoryStream();
            protbuf.WriteTo(stream);
            var buf = stream.ToArray();
            return new PduCommBinaryData(buf);
        }

        public void ConvertToPduData(IPduCommData src, IPduReader dst)
        {
            PduCommBinaryData binary = null;

            if (src.GetType() == typeof(PduCommBinaryData))
            {
                binary = (PduCommBinaryData)src;
            }
            if (binary == null)
            {
                throw new ArgumentException("Invalid data type");
            }

            var parser = new MessageParser<Ev3PduActuator>(() => new Ev3PduActuator());
            var packet = parser.ParseFrom(new MemoryStream(binary.GetData()));
            dst.GetWriteOps().SetHeaderData("header", packet.Header.Name);
            dst.GetWriteOps().SetHeaderData("version", packet.Header.Version);
            dst.GetWriteOps().SetHeaderData("simulation_time", (long)packet.Header.AssetTime);
            dst.GetWriteOps().SetHeaderData("ext_off", packet.Header.ExtOff);
            dst.GetWriteOps().SetHeaderData("ext_size", packet.Header.ExtSize);

            dst.GetWriteOps().SetData("led", packet.Body.Leds.ToByteArray());
            dst.GetWriteOps().SetData("motor_power_a", packet.Body.Motors[0].Power);
            dst.GetWriteOps().SetData("motor_power_b", packet.Body.Motors[1].Power);
            dst.GetWriteOps().SetData("motor_power_c", packet.Body.Motors[2].Power);

            dst.GetWriteOps().SetData("motor_stop_a", packet.Body.Motors[0].Stop);
            dst.GetWriteOps().SetData("motor_stop_b", packet.Body.Motors[1].Stop);
            dst.GetWriteOps().SetData("motor_stop_c", packet.Body.Motors[2].Stop);

            dst.GetWriteOps().SetData("motor_reset_angle_a", packet.Body.Motors[0].ResetAngle);
            dst.GetWriteOps().SetData("motor_reset_angle_b", packet.Body.Motors[1].ResetAngle);
            dst.GetWriteOps().SetData("motor_reset_angle_c", packet.Body.Motors[2].ResetAngle);

            dst.GetWriteOps().SetData("gyro_reset", packet.Body.GyroReset);
            //SimpleLogger.Get().Log(Level.INFO, "ConvertToPduData: simulation_time=" + packet.Header.AssetTime);
            //SimpleLogger.Get().Log(Level.INFO, "ConvertToPduData: PDU:simulation_time=" + dst.GetReadOps().GetHeaderData("simulation_time"));
        }
    }
}
