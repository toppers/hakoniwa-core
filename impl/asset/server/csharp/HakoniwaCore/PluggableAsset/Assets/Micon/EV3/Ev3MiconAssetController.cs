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

        public Ev3MiconAssetController(string name)
        {
            this.name = name;
        }
        public long GetSimTime()
        {
            return this.connector.Reader.GetPdu().GetHeaderData("simulation_time");
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
            this.connector.Writer.GetPdu().SetHeaderData("hakoniwa_time", time);
        }

        public void RecvPdu()
        {
            this.connector.Reader.Recv();
        }

        public void SendPdu()
        {
            this.connector.Writer.Send();
        }
    }
}
