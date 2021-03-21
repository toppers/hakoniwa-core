﻿using Hakoniwa.PluggableAsset.Assets;
using Hakoniwa.PluggableAsset.Assets.Micon.EV3;
using Hakoniwa.PluggableAsset.Communication.Channel;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Method;
using Hakoniwa.PluggableAsset.Communication.Method.Udp;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using Hakoniwa.PluggableAsset.Communication.Pdu.Ev3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.PluggableAsset
{
    public class AssetConfigLoader
    {
        public static CoreConfig core_config;
        private static List<IInsideAssetController> inside_assets = new List<IInsideAssetController>();
        private static List<IOutsideAssetController> outside_assets = new List<IOutsideAssetController>();
        private static List<IPduReader> pdu_readers = new List<IPduReader>();
        private static List<IPduWriter> pdu_writers = new List<IPduWriter>();
        private static List<IIoReader> io_readers = new List<IIoReader>();
        private static List<IIoWriter> io_writers = new List<IIoWriter>();
        private static List<WriterConnector> writer_connectors = new List<WriterConnector>();
        private static List<ReaderConnector> reader_connectors = new List<ReaderConnector>();
        private static List<PduChannelConnector> pdu_channel_connectors = new List<PduChannelConnector>();
        private static List<PduIoConnector> pdu_io_connectors = new List<PduIoConnector>();

        private static PduChannelConnector GetPduChannelConnector(string name)
        {
            foreach (var e in pdu_channel_connectors)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        private static PduIoConnector GetPduIoConnector(string name)
        {
            foreach (var e in pdu_io_connectors)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        private static IIoReader GetIoReader(string name)
        {
            foreach (var e in io_readers)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        private static IIoWriter GetIoWriter(string name)
        {
            foreach (var e in io_writers)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        private static IPduReader GetIpduReader(string name)
        {
            foreach(var e in pdu_readers)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        private static IPduWriter GetIpduWriter(string name)
        {
            foreach (var e in pdu_writers)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        private static WriterConnector GetWriterConnector(string name)
        {
            foreach (var e in writer_connectors)
            {
                if (e.Name.Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        private static ReaderConnector GetReaderConnector(string name)
        {
            foreach (var e in reader_connectors)
            {
                if (e.Name.Equals(name))
                {
                    return e;
                }
            }
            return null;
        }

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
        public static void Load(string filepath)
        {
            Console.WriteLine("Hello---:" + filepath);
            try
            {
                string jsonString = File.ReadAllText(filepath);
                core_config = JsonConvert.DeserializeObject<CoreConfig>(jsonString);
                Console.WriteLine("jsonstring=" + jsonString);
                Console.WriteLine("core_config=" + core_config);
            }
            catch (Exception e)
            {
                Console.WriteLine("Not found config file:" + e);
                throw e;
            }
            //writer pdu configs
            foreach (var pdu in core_config.pdu_writers)
            {
                IPduWriter ipdu = null;
                if (pdu.class_name.Equals("Ev3PduWriter"))
                {
                    ipdu = new Ev3PduWriter(pdu.name);
                }
                if (ipdu == null)
                {
                    throw new InvalidDataException("ERROR: can not found classname=" + pdu.class_name);
                }
                AssetConfigLoader.pdu_writers.Add(ipdu);
            }
            //reader pdu configs
            foreach (var pdu in core_config.pdu_readers)
            {
                IPduReader ipdu = null;
                if (pdu.class_name.Equals("Ev3PduReader"))
                {
                    ipdu = new Ev3PduReader(pdu.name);
                }
                if (ipdu == null)
                {
                    throw new InvalidDataException("ERROR: can not found classname=" + pdu.class_name);
                }
                AssetConfigLoader.pdu_readers.Add(ipdu);
            }
            //udp method configs
            foreach (var method in core_config.udp_methods)
            {
                var config = new UdpConfig();
                config.IoSize = method.iosize;
                config.IpAddr = method.ipaddr;
                config.Portno = method.portno;
                if (method.is_read)
                {
                    var real_method = new UdpReader();
                    real_method.Initialize(config);
                    real_method.Name = method.method_name;
                    AssetConfigLoader.io_readers.Add(real_method);
                }
                else
                {
                    var real_method = new UdpWriter();
                    real_method.Initialize(config);
                    real_method.Name = method.method_name;
                    AssetConfigLoader.io_writers.Add(real_method);
                }
            }

            //reader connectors configs
            foreach (var connector in core_config.reader_connectors)
            {
                var method = AssetConfigLoader.GetIoReader(connector.method_name);
                if (method == null)
                {
                    throw new InvalidDataException("ERROR: can not found connector method_name=" + connector.method_name);
                }
                var pdu = AssetConfigLoader.GetIpduReader(connector.pdu_name);
                if (pdu == null)
                {
                    throw new InvalidDataException("ERROR: can not found connector pdu_name=" + connector.pdu_name);
                }
                var real_connector = ReaderConnector.Create(pdu, new ReaderChannel(method));
                real_connector.Name = connector.name;
                AssetConfigLoader.reader_connectors.Add(real_connector);
            }
            //writer connectors configs
            foreach (var connector in core_config.writer_connectors)
            {
                var method = AssetConfigLoader.GetIoWriter(connector.method_name);
                if (method == null)
                {
                    throw new InvalidDataException("ERROR: can not found connector method_name=" + connector.method_name);
                }
                var pdu = AssetConfigLoader.GetIpduWriter(connector.pdu_name);
                if (pdu == null)
                {
                    throw new InvalidDataException("ERROR: can not found connector pdu_name=" + connector.pdu_name);
                }
                var real_connector = WriterConnector.Create(pdu, new WriterChannel(method));
                real_connector.Name = connector.name;
                AssetConfigLoader.writer_connectors.Add(real_connector);
            }
            //pdu channel connectors configs
            foreach (var connector in core_config.pdu_channel_connectors)
            {
                var real_connector = PduChannelConnector.Create(connector.outside_asset_name);
                var reader = AssetConfigLoader.GetReaderConnector(connector.reader_connector_name);
                if (reader == null)
                {
                    throw new InvalidDataException("ERROR: can not found pdu channel connector reader=" + connector.reader_connector_name);
                }
                var writer = AssetConfigLoader.GetWriterConnector(connector.writer_connector_name);
                if (writer == null)
                {
                    throw new InvalidDataException("ERROR: can not found pdu channel connector writer=" + connector.writer_connector_name);
                }
                real_connector.Writer = writer;
                real_connector.Reader = reader;
                AssetConfigLoader.pdu_channel_connectors.Add(real_connector);
            }
            //inside asset configs
            foreach(var asset in core_config.inside_assets)
            {
                var connector = PduIoConnector.Create(asset.name);
                foreach(var name in asset.pdu_writer_names)
                {
                    var pdu = AssetConfigLoader.GetIpduWriter(name);
                    if (pdu == null)
                    {
                        throw new InvalidDataException("ERROR: can not found inside asset pdu writer=" + name);
                    }
                    connector.AddWriter(pdu);
                }
                foreach (var name in asset.pdu_reader_names)
                {
                    var pdu = AssetConfigLoader.GetIpduReader(name);
                    if (pdu == null)
                    {
                        throw new InvalidDataException("ERROR: can not found inside asset pdu reader=" + name);
                    }
                    connector.AddReader(pdu);
                }
            }

            //outside asset configs
            foreach (var asset in core_config.outside_assets)
            {
                IOutsideAssetController controller = null;
                if (asset.class_name.Equals("Ev3MiconAssetController"))
                {
                    controller = new Ev3MiconAssetController("Athrill");
                }
                if (controller == null)
                {
                    throw new InvalidDataException("ERROR: can not found classname=" + asset.class_name);
                }
                AssetConfigLoader.AddOutsideAsset(controller);
            }
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
            config.IpAddr = "172.20.172.177";
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
            config.IpAddr = "172.20.160.1";
            config.Portno = 54001;
            IIoReader reader = new UdpReader();
            reader.Initialize(config);

            //Channel
            ReaderChannel rchannel = new ReaderChannel(reader);

            var rconnector = ReaderConnector.Create(rpdu, rchannel);

            var athrill_connector = PduChannelConnector.Create("Athrill");
            athrill_connector.Writer = wconnector;
            athrill_connector.Reader = rconnector;

            AssetConfigLoader.AddOutsideAsset(new Ev3MiconAssetController("Athrill"));

            var robo_connector = PduIoConnector.Create("RoboModel");
            robo_connector.AddWriter(wpdu);
            robo_connector.AddReader(rpdu);
        }
    }
}
