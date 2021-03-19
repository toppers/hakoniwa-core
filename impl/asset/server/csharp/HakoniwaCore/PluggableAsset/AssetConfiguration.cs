using Hakoniwa.PluggableAsset.Assets;
using Hakoniwa.PluggableAsset.Assets.Micon.EV3;
using Hakoniwa.PluggableAsset.Communication.Channel;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Method;
using Hakoniwa.PluggableAsset.Communication.Method.Udp;
using Hakoniwa.PluggableAsset.Communication.Pdu.Ev3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset
{
    class AssetConfiguration
    {
        private static List<IInsideAssetController> inside_assets = new List<IInsideAssetController>();
        private static List<IOutsideAssetController> outside_assets = new List<IOutsideAssetController>();

        public static void AddInsideAsset(IInsideAssetController asset)
        {
            inside_assets.Add(asset);
        }
        public static void AddOutsideAsset(IOutsideAssetController asset)
        {
            outside_assets.Add(asset);
        }
        public static IInsideAssetController GetInsideAsset(string name)
        {
            foreach(var e in inside_assets)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        public static IOutsideAssetController GetOutsideAsset(string name)
        {
            foreach (var e in outside_assets)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        public static void Load()
        {
            LoadConfig();
        }
        private static void LoadConfig()
        {
            /***********************************
             * 
             *     RoboModel(inner) ==> Athrill(outer)
             * 
             ***********************************/
            //Pdu
            Ev3PduWriter wpdu = new Ev3PduWriter("Ev3SensorPdu");

            //Method
            UdpConfig config = new UdpConfig();
            config.IoSize = 1024;
            config.IpAddr = "172.30.24.61";
            config.Portno = 54002;
            IIoWriter writer = new UdpWriter();
            writer.Initialize(config);

            //Channel
            WriterChannel wchannel = new WriterChannel(writer);

            var wconnector = WriterConnector.Create(wpdu, wchannel);

            /***********************************
             * 
             *     Athrill(outer) ==> RoboModel(inner)
             * 
             ***********************************/
            //Pdu
            Ev3PduReader rpdu = new Ev3PduReader("Ev3ActuatorPdu");

            //Method
            config = new UdpConfig();
            config.IoSize = 1024;
            config.IpAddr = "172.30.16.1";
            config.Portno = 54001;
            IIoReader reader = new UdpReader();
            reader.Initialize(config);

            //Channel
            ReaderChannel rchannel = new ReaderChannel(reader);

            var rconnector = ReaderConnector.Create(rpdu, rchannel);

            var athrill_connector = PduChannelConnector.Create("Athrill");
            athrill_connector.Writer = wconnector;
            athrill_connector.Reader = rconnector;

            AssetConfiguration.AddOutsideAsset(new Ev3MiconAssetController("Athrill"));

            var robo_connector = PduIoConnector.Create("RoboModel");
            robo_connector.AddWriter(wpdu);
            robo_connector.AddReader(rpdu);
        }
    }
}
