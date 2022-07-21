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
                    Name = src.GetReadOps().Ref("head").GetDataString("name"),
                    AssetTime = (ulong)src.GetReadOps().Ref("head").GetDataInt64("asset_time"),
                    Version = src.GetReadOps().Ref("head").GetDataUInt32("version"),
                    ExtOff = src.GetReadOps().Ref("head").GetDataUInt32("ext_off"),
                    ExtSize = src.GetReadOps().Ref("head").GetDataUInt32("ext_size"),
                },
                Body = new Ev3PduActuator.Types.Body
                {
                    Leds = ByteString.CopyFrom(src.GetReadOps().GetDataUInt8Array("leds")),
                    Motors =
                    {
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = src.GetReadOps().Refs("motors")[0].GetDataInt32("power"),
                            Stop = src.GetReadOps().Refs("motors")[0].GetDataUInt32("stop"),
                            ResetAngle = src.GetReadOps().Refs("motors")[0].GetDataUInt32("reset_angle"),
                        },
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = src.GetReadOps().Refs("motors")[1].GetDataInt32("power"),
                            Stop = src.GetReadOps().Refs("motors")[1].GetDataUInt32("stop"),
                            ResetAngle = src.GetReadOps().Refs("motors")[1].GetDataUInt32("reset_angle"),
                        },
                        new Ev3PduActuator.Types.Body.Types.Motor {
                            Power = src.GetReadOps().Refs("motors")[2].GetDataInt32("power"),
                            Stop = src.GetReadOps().Refs("motors")[2].GetDataUInt32("stop"),
                            ResetAngle = src.GetReadOps().Refs("motors")[2].GetDataUInt32("reset_angle"),
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

            if (src is PduCommBinaryData)
            {
                binary = (PduCommBinaryData)src;
            }
            if (binary == null)
            {
                throw new ArgumentException("Invalid data type");
            }

            var parser = new MessageParser<Ev3PduActuator>(() => new Ev3PduActuator());
            var packet = parser.ParseFrom(new MemoryStream(binary.GetData()));
            dst.GetWriteOps().Ref("head").SetData("name", packet.Header.Name);
            dst.GetWriteOps().Ref("head").SetData("version", packet.Header.Version);
            dst.GetWriteOps().Ref("head").SetData("asset_time", (long)packet.Header.AssetTime);
            dst.GetWriteOps().Ref("head").SetData("ext_off", packet.Header.ExtOff);
            dst.GetWriteOps().Ref("head").SetData("ext_size", packet.Header.ExtSize);

            dst.GetWriteOps().SetData("leds", packet.Body.Leds.ToByteArray());

            for (int i = 0; i < 3; i++)
            {
                dst.GetWriteOps().Refs("motors")[i].SetData("power", packet.Body.Motors[i].Power);
                dst.GetWriteOps().Refs("motors")[i].SetData("stop", packet.Body.Motors[i].Stop);
                dst.GetWriteOps().Refs("motors")[i].SetData("reset_angle", packet.Body.Motors[i].ResetAngle);
            }

            dst.GetWriteOps().SetData("gyro_reset", packet.Body.GyroReset);
        }
    }
}
