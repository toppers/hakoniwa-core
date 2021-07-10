using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Assets;
using Hakoniwa.PluggableAsset.Assets.Micon.EV3;
using Hakoniwa.PluggableAsset.Assets.Robot.EV3;
using Hakoniwa.PluggableAsset.Communication.Channel;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Method;
using Hakoniwa.PluggableAsset.Communication.Method.Mmap;
using Hakoniwa.PluggableAsset.Communication.Method.ROS;
using Hakoniwa.PluggableAsset.Communication.Method.Udp;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using Hakoniwa.PluggableAsset.Communication.Pdu.Ev3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        private static List<Pdu> pdus = new List<Pdu>();
        private static List<PduDataConfig> pdu_configs = new List<PduDataConfig>();

        public static Pdu GetPdu(string name)
        {
            foreach (var e in pdus)
            {
                if (e.GetName().Equals(name))
                {
                    return e;
                }
            }
            return null;
        }

        public static RosTopicMessageConfig GetRosTopic(string config_name)
        {
            if (core_config.ros_topics == null)
            {
                throw new ArgumentException("Can not found ros_topics");
            }
            foreach(var e in core_config.ros_topics)
            {
                if (e.topic_message_name.Equals(config_name))
                {
                    return e;
                }
            }
            return null;
        }

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
        public static List<PduChannelConnector> RefPduChannelConnector()
        {
            return pdu_channel_connectors;
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
            if (io_readers == null)
            {
                return null;
            }
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
            if (io_writers == null)
            {
                return null;
            }
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
            foreach (var e in pdu_readers)
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

        public static PduDataConfig GetPduConfig(string arg_pdu_type_name)
        {
            foreach (var e in AssetConfigLoader.pdu_configs)
            {
                if (e.pdu_type_name.Equals(arg_pdu_type_name))
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
            foreach (var e in inside_assets)
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

        private static Type GetType(string path, string class_name)
        {
            Type typeinfo = null;
            SimpleLogger.Get().Log(Level.INFO, "path=" + path + " class_name=" + class_name);
            if (path == null)
            {
                try
                {
                    typeinfo = Type.GetType(class_name);
                    if (typeinfo == null)
                    {
                        //see:https://freelyapps.net/unityengine-types-can-no-longer-be-used/
                        typeinfo = System.Reflection.Assembly.Load("Assembly-CSharp").GetType(class_name);
                        if (typeinfo == null)
                        {
                            throw new InvalidDataException("ERROR: can not found class=" + class_name);
                        }
                    }
                    SimpleLogger.Get().Log(Level.INFO, "load typeinfo" + typeinfo);
                }
                catch (Exception)
                {
                    throw new InvalidDataException("ERROR: can not found class_name=" + class_name);
                }
            }
            else
            {
                var asm = Assembly.LoadFrom(path);
                typeinfo = asm.GetType(class_name);
                if (typeinfo == null)
                {
                    throw new InvalidDataException("ERROR: can not found class=" + class_name);
                }
            }
            return typeinfo;
        }

        private static object ClassLoader(string path, string class_name, string arg1)
        {
            Type typeinfo = AssetConfigLoader.GetType(path, class_name);
            if (arg1 == null)
            {
                SimpleLogger.Get().Log(Level.INFO, "activate class_name=" + class_name);
                return Activator.CreateInstance(typeinfo);
            }
            else
            {
                return Activator.CreateInstance(typeinfo, arg1);

            }
        }
        private static void LoadPduConfig(PduDataConfig config)
        {
            PduDataFieldsConfig cfg = null;
            if (config.pdu_data_field_path != null)
            {
                string jsonString = File.ReadAllText(config.pdu_data_field_path);
                cfg = JsonConvert.DeserializeObject<PduDataFieldsConfig>(jsonString);
                config.fields = cfg.fields;
            }
            SimpleLogger.Get().Log(Level.DEBUG, "LoadPduConfig(): pdu_type_name=" + config.pdu_type_name);
            AssetConfigLoader.pdu_configs.Add(config);
        }
        private static void LoadPdu(PduDataConfig config)
        {
            PduDataFieldsConfig cfg = new PduDataFieldsConfig();
            cfg.fields = config.fields;
            Pdu pdu = new Pdu(config.pdu_type_name, cfg);
            SimpleLogger.Get().Log(Level.DEBUG, "LoadPdu(): type=" + pdu.GetName() + " pdu_type_name=" + config.pdu_type_name);
            AssetConfigLoader.pdus.Add(pdu);
        }
        public static void Load(string filepath)
        {
            try
            {
                string jsonString = File.ReadAllText(filepath);
                core_config = JsonConvert.DeserializeObject<CoreConfig>(jsonString);
                SimpleLogger.Get().Log(Level.INFO, "jsonstring=" + jsonString);
            }
            catch (Exception e)
            {
                SimpleLogger.Get().Log(Level.ERROR, e);
                throw e;
            }
            if (core_config.pdu_configs != null)
            {
                foreach (var cfg in core_config.pdu_configs)
                {
                    AssetConfigLoader.LoadPduConfig(cfg);
                }
                foreach (var cfg in AssetConfigLoader.pdu_configs)
                {
                    AssetConfigLoader.LoadPdu(cfg);
                }
            }
            if (core_config.ros_topics_path != null)
            {
                string jsonString = File.ReadAllText(core_config.ros_topics_path);
                var container = JsonConvert.DeserializeObject<RosTopicMessageConfigContainer>(jsonString);
                core_config.ros_topics = container.fields;
            }
            //writer pdu configs
            foreach (var pdu in core_config.pdu_writers)
            {
                IPduWriter ipdu = null;
                if (pdu.topic_message_name == null)
                {
                    Pdu pdu_data = AssetConfigLoader.GetPdu(pdu.pdu_config_name);
                    if (pdu_data == null)
                    {
                        throw new InvalidDataException("ERROR: can not found pdu=" + pdu.name);
                    }
                    Type typeinfo = AssetConfigLoader.GetType(pdu.path, pdu.class_name);
                    ipdu = Activator.CreateInstance(typeinfo, pdu_data, pdu.name) as IPduWriter;
                    SimpleLogger.Get().Log(Level.INFO, "pdu writer loaded:" + pdu.class_name);
                    if (ipdu == null)
                    {
                        throw new InvalidDataException("ERROR: can not found classname=" + pdu.class_name);
                    }
                    if (pdu.conv_class_name != null)
                    {
                        IPduWriterConverter conv = AssetConfigLoader.ClassLoader(null, pdu.conv_class_name, null) as IPduWriterConverter;
                        ipdu.SetConverter(conv);
                    }
                }
                else
                {
                    Pdu topic = AssetConfigLoader.GetPdu(pdu.pdu_config_name);
                    if (topic == null)
                    {
                        throw new InvalidDataException("ERROR: can not found pdu=" + pdu.name);
                    }
                    RosTopicMessageConfig cfg = AssetConfigLoader.GetRosTopic(pdu.topic_message_name);
                    Type typeinfo = AssetConfigLoader.GetType(pdu.path, pdu.class_name);
                    ipdu = Activator.CreateInstance(typeinfo, topic, pdu.name, cfg.topic_message_name, cfg.topic_type_name) as IPduWriter;
                    if (pdu.conv_class_name != null)
                    {
                        IPduWriterConverter conv = AssetConfigLoader.ClassLoader(null, pdu.conv_class_name, null) as IPduWriterConverter;
                        ipdu.SetConverter(conv);
                    }
                    else
                    {
                        throw new InvalidDataException("ERROR: can not found conv_class_name=" + pdu.conv_class_name);
                    }
                }
                if (ipdu != null)
                {
                    SimpleLogger.Get().Log(Level.DEBUG, "pdu_writer: " + ipdu.GetName());
                    AssetConfigLoader.pdu_writers.Add(ipdu);
                }
            }
            //reader pdu configs
            foreach (var pdu in core_config.pdu_readers)
            {
                IPduReader ipdu = null;
                if (pdu.topic_message_name == null)
                {
                    Pdu pdu_data = AssetConfigLoader.GetPdu(pdu.pdu_config_name);
                    if (pdu_data == null)
                    {
                        throw new InvalidDataException("ERROR: can not found pdu=" + pdu.name);
                    }
                    Type typeinfo = AssetConfigLoader.GetType(pdu.path, pdu.class_name);
                    ipdu = Activator.CreateInstance(typeinfo, pdu_data, pdu.name) as IPduReader;
                    SimpleLogger.Get().Log(Level.INFO, "pdu writer loaded:" + pdu.class_name);
                    if (ipdu == null)
                    {
                        throw new InvalidDataException("ERROR: can not found classname=" + pdu.class_name);
                    }
                    if (pdu.conv_class_name != null)
                    {
                        IPduReaderConverter conv = AssetConfigLoader.ClassLoader(pdu.conv_path, pdu.conv_class_name, null) as IPduReaderConverter;
                        ipdu.SetConverter(conv);
                    }
                }
                else
                {
                    Pdu topic = AssetConfigLoader.GetPdu(pdu.pdu_config_name);
                    if (topic == null)
                    {
                        throw new InvalidDataException("ERROR: can not found pdu=" + pdu.name);
                    }
                    SimpleLogger.Get().Log(Level.INFO, "ros topic pdu reader name = " + pdu.name);
                    RosTopicMessageConfig cfg = AssetConfigLoader.GetRosTopic(pdu.topic_message_name);
                    Type typeinfo = AssetConfigLoader.GetType(pdu.path, pdu.class_name);
                    ipdu = Activator.CreateInstance(typeinfo, topic, pdu.name, cfg.topic_message_name, cfg.topic_type_name) as IPduReader;
                    if (pdu.conv_class_name != null)
                    {
                        IPduReaderConverter conv = AssetConfigLoader.ClassLoader(null, pdu.conv_class_name, null) as IPduReaderConverter;
                        ipdu.SetConverter(conv);
                    }
                    else
                    {
                        throw new InvalidDataException("ERROR: can not found conv_class_name=" + pdu.conv_class_name);
                    }
                }
                if (ipdu != null)
                {
                    SimpleLogger.Get().Log(Level.INFO, "pdu reader name = " + ipdu.GetName());
                    AssetConfigLoader.pdu_readers.Add(ipdu);
                }
            }
            if (core_config.udp_methods != null)
            {
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
                        SimpleLogger.Get().Log(Level.INFO, "UdpMethod : " + real_method.Name + ": " + config.IpAddr + ": " + config.Portno);
                    }
                }
            }
            if (core_config.mmap_methods != null)
            {
                //mmap method configs
                foreach (var method in core_config.mmap_methods)
                {
                    var config = new MmapConfig();
                    config.io_size = method.iosize;
                    config.filepath = method.filepath;
                    if (method.is_read)
                    {
                        var real_method = new MmapReader();
                        real_method.Initialize(config);
                        real_method.Name = method.method_name;
                        AssetConfigLoader.io_readers.Add(real_method);
                    }
                    else
                    {
                        var real_method = new MmapWriter();
                        real_method.Initialize(config);
                        real_method.Name = method.method_name;
                        AssetConfigLoader.io_writers.Add(real_method);
                    }
                }
            }
            if (core_config.ros_topic_method != null)
            {
                IRosTopicIo ros_topic_io = AssetConfigLoader.ClassLoader(core_config.ros_topic_method.path, 
                    core_config.ros_topic_method.class_name, null) as IRosTopicIo;
                if (ros_topic_io == null)
                {
                    throw new InvalidDataException("ERROR: can not found classname=" + core_config.ros_topic_method.class_name);
                }
                SimpleLogger.Get().Log(Level.INFO, "ros topic io loaded:" + core_config.ros_topic_method.class_name);

                RosTopicConfig config = new RosTopicConfig(core_config.ros_topic_method.name, ros_topic_io);
                RosTopicWriter writer = new RosTopicWriter(config);
                AssetConfigLoader.io_writers.Add(writer);

                RosTopicReader reader = new RosTopicReader(config);
                SimpleLogger.Get().Log(Level.INFO, "ros topic reader name=" + reader.GetName());
                AssetConfigLoader.io_readers.Add(reader);
            }


            //reader connectors configs
            foreach (var connector in core_config.reader_connectors)
            {
                SimpleLogger.Get().Log(Level.INFO, "reader connector method_name=" + connector.method_name);
                var method = AssetConfigLoader.GetIoReader(connector.method_name);
                if (method == null)
                {
                    throw new InvalidDataException("ERROR: can not found connector method_name=" + connector.method_name);
                }
                SimpleLogger.Get().Log(Level.INFO, "reader connector pdu_name=" + connector.pdu_name);
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
                var w_pdu = AssetConfigLoader.GetIpduWriter(connector.pdu_name);
                var r_pdu = AssetConfigLoader.GetIpduReader(connector.pdu_name);
                if ((w_pdu == null) && (r_pdu == null))
                {
                    throw new InvalidDataException("ERROR: can not found connector pdu_name=" + connector.pdu_name);
                }
                if (w_pdu != null)
                {
                    var real_connector = WriterConnector.Create(w_pdu, new WriterChannel(method));
                    real_connector.Name = connector.name;
                    AssetConfigLoader.writer_connectors.Add(real_connector);
                }
                else
                {
                    var real_connector = WriterConnector.Create(r_pdu, new WriterChannel(method));
                    real_connector.Name = connector.name;
                    AssetConfigLoader.writer_connectors.Add(real_connector);
                }
            }
            //pdu channel connectors configs
            foreach (var connector in core_config.pdu_channel_connectors)
            {
                var real_connector = PduChannelConnector.Create(connector.outside_asset_name);
                var reader = AssetConfigLoader.GetReaderConnector(connector.reader_connector_name);
                var writer = AssetConfigLoader.GetWriterConnector(connector.writer_connector_name);
                if ((reader == null) && (writer == null))
                {
                    throw new InvalidDataException("ERROR: can not found pdu channel connector writer=" + connector.writer_connector_name + " reader="+connector.reader_connector_name);
                }
                real_connector.Writer = writer;
                real_connector.Reader = reader;
                AssetConfigLoader.pdu_channel_connectors.Add(real_connector);
                SimpleLogger.Get().Log(Level.INFO, "PduChannelConnector :" + connector.writer_connector_name);
            }
            //inside asset configs
            if (core_config.inside_assets != null)
            {
                foreach (var asset in core_config.inside_assets)
                {
                    var connector = PduIoConnector.Create(asset.name);
                    foreach (var name in asset.pdu_writer_names)
                    {
                        var pdu = AssetConfigLoader.GetIpduWriter(name);
                        if (pdu == null)
                        {
                            throw new InvalidDataException("ERROR: can not found inside asset pdu writer=" + name);
                        }
                        SimpleLogger.Get().Log(Level.DEBUG, asset.name + " :pdu io connect add writer:" + pdu.GetName());
                        connector.AddWriter(pdu);
                    }
                    foreach (var name in asset.pdu_reader_names)
                    {
                        var pdu = AssetConfigLoader.GetIpduReader(name);
                        if (pdu == null)
                        {
                            throw new InvalidDataException("ERROR: can not found inside asset pdu reader=" + name);
                        }
                        SimpleLogger.Get().Log(Level.DEBUG, asset.name + " pdu io connect add reader:" + pdu.GetName());
                        connector.AddReader(pdu);
                    }
                }
            }
            //outside asset configs
            if (core_config.outside_assets != null)
            {
                foreach (var asset in core_config.outside_assets)
                {
                    IOutsideAssetController controller = null;
                    if (asset.class_name != null)
                    {
                        controller = AssetConfigLoader.ClassLoader(null, asset.class_name, asset.name) as IOutsideAssetController;
                    }
                    if (controller == null)
                    {
                        throw new InvalidDataException("ERROR: can not found classname=" + asset.class_name);
                    }
                    SimpleLogger.Get().Log(Level.INFO, "OutSideAsset :" + asset.name);
                    AssetConfigLoader.AddOutsideAsset(controller);
                }
            }
        }
    }
}
