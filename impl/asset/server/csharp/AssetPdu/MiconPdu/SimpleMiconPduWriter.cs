using Google.Protobuf;
using Hakoniwa.PluggableAsset.Communication.Method;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Micon
{
    class SimpleMiconPduWriter : IPduWriter
    {
        private MiconPduSensor packet = null;
        private string name;
        public SimpleMiconPduWriter(string name)
        {
            this.name = name;
            this.packet = new MiconPduSensor()
            {
                Header = new MiconPduSensor.Types.Header
                {
                    Name = name,
                    Version = 0x1,
                    HakoniwaTime = 0,
                }
            };
        }
        public string GetName()
        {
            return name;
        }

        public void Send(IIoWriter writer)
        {
            packet.Header.Name = name;
            var stream = new MemoryStream();
            this.packet.WriteTo(stream);
            var buf = stream.ToArray();
            writer.Flush(ref buf);
        }

        public void SetData(string field_name, int value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, ulong value)
        {
            throw new NotImplementedException();
        }

        public void SetData(string field_name, double value)
        {
            throw new NotImplementedException();
        }

        public void SetHeaderData(string field_name, long value)
        {
            if (field_name.Equals("hakonwia_time"))
            {
                this.packet.Header.HakoniwaTime = (ulong)value;
            }
        }
    }
}
