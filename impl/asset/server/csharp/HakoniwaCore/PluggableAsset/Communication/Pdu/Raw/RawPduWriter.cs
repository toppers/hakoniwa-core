using System;
namespace Hakoniwa.PluggableAsset.Communication.Pdu.Raw
{
    class RawPduWriter : IPduWriter
    {
        private string name;
        private Pdu pdu;
        private IPduWriterConverter converter = null;

        public RawPduWriter(Pdu arg_pdu, string name)
        {
            this.name = name;
            this.pdu = arg_pdu;
        }
        public IPduCommData Get()
        {
            if (this.converter == null)
            {
                throw new ArgumentException("Converter is not set");
            }
            else
            {
                //TODO ロボットの書き込み周期に合わせて結果を変えたい。
                //書き込み周期と一致：現行通り
                //書き込み周期と不一致：nullを返す
                return this.converter.ConvertToIoData(this);
            }
        }

        public string GetName()
        {
            return this.name;
        }
        public IPduReadOperation GetReadOps()
        {
            return this.pdu.GetPduReadOps();
        }

        public IPduWriteOperation GetWriteOps()
        {
            return this.pdu.GetPduWriteOps();
        }

        public void SetConverter(IPduWriterConverter cnv)
        {
            this.converter = cnv;
        }
        public void Reset()
        {
            this.pdu.Reset();
        }
    }
}
