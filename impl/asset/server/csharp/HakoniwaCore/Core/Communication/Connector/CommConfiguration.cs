using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Communication.Channel;
using Hakoniwa.Core.Communication.Method;
using Hakoniwa.Core.Communication.Method.Udp;
using Hakoniwa.Core.Communication.Pdu.Ev3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Connector
{
    class CommConfiguration
    {
        public static void Load()
        {
            LoadPublishers();
            LoadSubscribers();
        }
        private static void LoadPublishers()
        {
            /***********************************
             * 
             *     RoboModel(inner) ==> Athrill(outer)
             * 
             ***********************************/
            //Pdu
            Ev3PduWriter pdu = new Ev3PduWriter();

            //Method
            UdpWriterConfig config = new UdpWriterConfig();
            config.io_size = 1024;
            config.ipaddr = "127.0.0.1";
            config.portno = 50004;
            IIoWriter writer = new UdpWriter();
            writer.Initialize(config);

            //Channel
            WriterChannel channel = new WriterChannel(writer);

            var connector = WriterConnector.Create(pdu, channel);

            var publisher = Publisher.Create("RoboModel");
            publisher.GetConnectors().Add(connector);

        }
        private static void LoadSubscribers()
        {
            /***********************************
             * 
             *     Athrill(outer) ==> RoboModel(inner)
             * 
             ***********************************/
            //Pdu
            Ev3PduReader pdu = new Ev3PduReader();

            //Method
            UdpReaderConfig config = new UdpReaderConfig();
            config.io_size = 1024;
            config.ipaddr = "127.0.0.1";
            config.portno = 50005;
            IIoReader reader = new UdpReader();
            reader.Initialize(config);

            //Channel
            ReaderChannel channel = new ReaderChannel(reader);

            var connector = ReaderConnector.Create(pdu, channel);

            var subscriber = Subscriber.Create("RoboModel");
            subscriber.GetConnectors().Add(connector);

        }
    }
}
