using Hakoniwa.PluggableAsset.Communication.Connector;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Micon.EV3
{
    class Ev3MiconAssetController : IOutsideAssetController
    {
        private string name;
        private PduChannelConnector connector;

        public Ev3MiconAssetController(string asset_name)
        {
            this.name = asset_name;
        }

        public string GetName()
        {
            return this.name;
        }

        public long GetSimTime()
        {
            return this.connector.Reader.GetPdu().GetReadOps().Ref("head").GetDataInt64("asset_time");
        }

        public void Initialize()
        {
            this.connector = PduChannelConnector.Get(this.name);
        }

        public bool IsConnected()
        {
            return this.connector.Reader.GetPdu().IsValidData();
        }

        public void PutHakoniwaTime(long time)
        {
            this.connector.Writer.GetPdu().GetWriteOps().Ref("head").SetData("hakoniwa_time", time);
        }

        public void RecvPdu()
        {
            this.connector.Reader.Recv();
        }

        public void SendPdu()
        {
            this.connector.Writer.SendWriterPdu();
        }
    }
}
