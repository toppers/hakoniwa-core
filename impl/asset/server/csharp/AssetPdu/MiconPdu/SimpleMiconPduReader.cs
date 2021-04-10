using Google.Protobuf;
using Hakoniwa.PluggableAsset.Communication.Method;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Micon
{
    class SimpleMiconPduReader : IPduReader
    {
        private MiconPduActuator packet = null;
        private string name;

        public SimpleMiconPduReader(string name)
        {
            this.name = name;
        }
        public void GetData(string field_name, out int value)
        {
            throw new NotImplementedException();
        }

        public void GetData(string field_name, out ulong value)
        {
            throw new NotImplementedException();
        }

        public void GetData(string field_name, out double value)
        {
            throw new NotImplementedException();
        }

        public long GetHeaderData(string field_name)
        {
            if (this.packet == null)
            {
                return 0;
            }
            else
            {
                return (long)packet.Header.AssetTime;
            }
        }

        public string GetName()
        {
            return name;
        }

        public bool IsValidData()
        {
            if (this.packet == null)
            {
                return false;
            }
            else
            {
                return (this.packet.Header.Version == 0x1);
            }
        }

        public void Recv(IIoReader reader)
        {
            var buf = reader.Recv();
            var parser = new MessageParser<MiconPduActuator>(() => new MiconPduActuator());
            this.packet = parser.ParseFrom(new MemoryStream(buf));
        }

        public void Send(IIoWriter writer)
        {
            if (this.packet != null)
            {
                var stream = new MemoryStream();
                this.packet.WriteTo(stream);
                var buf = stream.ToArray();
                writer.Flush(ref buf);
            }
        }
    }
}
