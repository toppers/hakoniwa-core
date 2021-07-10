using Hakoniwa.Core.Utils.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Ev3
{
    class Ev3PduReaderRawConverter : IPduReaderConverter
    {
        private byte[] my_buffer = new byte[1024];

        public Ev3PduReaderRawConverter()
        {

        }

        public void ConvertToPduData(IPduCommData src, IPduReader dst)
        {
            PduCommBinaryData src_data = src as PduCommBinaryData;
            byte[] buffer = src_data.GetData();

            //Actuator

            //header:name
            byte[] header_name = new byte[4];
            Buffer.BlockCopy(buffer, 0, header_name, 0, header_name.Length);
            //SimpleLogger.Get().Log(Level.DEBUG, "Ev3PduReaderRawConverter.ConvertToPduData: name=" + System.Text.Encoding.ASCII.GetString(header_name));
            dst.GetWriteOps().Ref("header").SetData("name", System.Text.Encoding.ASCII.GetString(header_name));
            //header:version
            dst.GetWriteOps().Ref("header").SetData("version", BitConverter.ToUInt32(buffer, 4));
            //header:asset_time
            dst.GetWriteOps().Ref("header").SetData("asset_time", BitConverter.ToInt64(buffer, 8));
            //header:ext_off
            dst.GetWriteOps().Ref("header").SetData("ext_off", BitConverter.ToUInt32(buffer, 24));
            //header:ext_size
            dst.GetWriteOps().Ref("header").SetData("ext_size", BitConverter.ToUInt32(buffer, 28));

            //body:led
            byte[] leds = new byte[1];
            uint uint_led = BitConverter.ToUInt32(buffer, (32 + 0));
            leds[0] = (byte)uint_led;
            dst.GetWriteOps().SetData("leds", leds);

            //body:motors
            dst.GetWriteOps().Refs("motors")[0].SetData("power", BitConverter.ToInt32(buffer, (32 + 4)));
            dst.GetWriteOps().Refs("motors")[1].SetData("power", BitConverter.ToInt32(buffer, (32 + 8)));
            dst.GetWriteOps().Refs("motors")[2].SetData("power", BitConverter.ToInt32(buffer, (32 + 12)));
            dst.GetWriteOps().Refs("motors")[0].SetData("stop", BitConverter.ToUInt32(buffer, (32 + 20)));
            dst.GetWriteOps().Refs("motors")[1].SetData("stop", BitConverter.ToUInt32(buffer, (32 + 24)));
            dst.GetWriteOps().Refs("motors")[2].SetData("stop", BitConverter.ToUInt32(buffer, (32 + 28)));
            dst.GetWriteOps().Refs("motors")[0].SetData("reset_angle", BitConverter.ToUInt32(buffer, (32 + 36)));
            dst.GetWriteOps().Refs("motors")[1].SetData("reset_angle", BitConverter.ToUInt32(buffer, (32 + 40)));
            dst.GetWriteOps().Refs("motors")[2].SetData("reset_angle", BitConverter.ToUInt32(buffer, (32 + 44)));

            //body:gyro_reset
            dst.GetWriteOps().SetData("gyro_reset", BitConverter.ToUInt32(buffer, (32 + 52)));
            return;
        }
        public IPduCommData ConvertToIoData(IPduReader src)
        {
            if (!src.IsValidData())
            {
                return null;
            }
            //Actuator
            //SimpleLogger.Get().Log(Level.DEBUG, "dbg1");
            //header:name
            var tmp_name = src.GetReadOps().Ref("header").GetDataString("name");
            //SimpleLogger.Get().Log(Level.DEBUG, "tmp_name=" + tmp_name);
            byte[] tmp_bytes = System.Text.Encoding.ASCII.GetBytes(tmp_name);
            //SimpleLogger.Get().Log(Level.DEBUG, "tmp_bytes.Length=" + tmp_bytes.Length);
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 0, 4);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg2");
            //header:version
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Ref("header").GetDataUInt32("version"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 4, tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg3");
            //header:asset_time
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Ref("header").GetDataInt64("asset_time"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 8, tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg4");
            //header:ext_off
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Ref("header").GetDataUInt32("ext_off"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 24, tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg5");
            //header:ext_size
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Ref("header").GetDataUInt32("ext_size"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, 28, tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg6");
            //body:led
            tmp_bytes = src.GetReadOps().GetDataUInt8Array("leds");
            uint tmp_led = tmp_bytes[0];
            tmp_bytes = BitConverter.GetBytes(tmp_led);
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 0), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg7");
            //body:motors
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[0].GetDataInt32("power"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 4), tmp_bytes.Length);
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[1].GetDataInt32("power"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 8), tmp_bytes.Length);
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[2].GetDataInt32("power"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 12), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg8");
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[0].GetDataUInt32("stop"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 20), tmp_bytes.Length);
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[1].GetDataUInt32("stop"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 24), tmp_bytes.Length);
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[2].GetDataUInt32("stop"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 28), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg9");
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[0].GetDataUInt32("reset_angle"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 36), tmp_bytes.Length);
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[1].GetDataUInt32("reset_angle"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 40), tmp_bytes.Length);
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().Refs("motors")[2].GetDataUInt32("reset_angle"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 44), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg10");
            //body:gyro_reset
            tmp_bytes = BitConverter.GetBytes(src.GetReadOps().GetDataUInt32("gyro_reset"));
            Buffer.BlockCopy(tmp_bytes, 0, this.my_buffer, (32 + 52), tmp_bytes.Length);

            //SimpleLogger.Get().Log(Level.DEBUG, "dbg11");
            return new PduCommBinaryData(this.my_buffer);
        }

    }
}
