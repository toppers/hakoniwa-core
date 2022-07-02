using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Hakoniwa.PluggableAsset
{
    [System.Serializable]
    public class CoreConfig
    {
        public string cpp_mode; // asset or master
        public string cpp_asset_name;
        public string core_ipaddr;
        public int core_portno;
        public long asset_timeout;
        public SimTimeSyncConfig sim_time_sync;
        public string SymTimeMeasureFilePath;
        public string inside_assets_path;
        public InsideAssetConfig []  inside_assets;

        public string outside_assets_path;
        public OutsideAssetConfig [] outside_assets;
        public string pdu_writers_path;
        public PduWriterConfig[] pdu_writers;
        public string pdu_readers_path;
        public PduReaderConfig[] pdu_readers;
        public string ros_topics_path;
        public RosTopicMessageConfig[] ros_topics;
        public string pdu_configs_path;
        public PduDataConfig[] pdu_configs;

        public string udp_methods_path;
        public UdpMethodConfig[] udp_methods;

        public string mmap_methods_path;
        public MmapMethodConfig[] mmap_methods;

        public string shm_methods_path;
        public ShmMethodConfig[] shm_methods;

        public string ros_topic_method_path;
        public RosTopicMethodConfig ros_topic_method;
        public string reader_connectors_path;
        public ReaderConnectorConfig[] reader_connectors;
        public string writer_connectors_path;
        public WriterConnectorConfig[] writer_connectors;
        public string pdu_channel_connectors_path;
        public PduChannelConnectorConfig[] pdu_channel_connectors;

        public string param_world_config_path;
        public ParamWorldConfigContainer param_world_config;
    }
    [System.Serializable]
    public class SimTimeSyncConfig
    {
        public long deltaTimeMsec; //msec
        public long maxDelayTimeMsec; //msec
    }
    [System.Serializable]
    public class InsideAssetConfig
    {
        public string name;
        public string [] pdu_writer_names;
        public string [] pdu_reader_names;
        public string core_class_name;
    }

    [System.Serializable]
    public class OutsideAssetConfig
    {
        public string name;
        public string class_name;
        public string path;
    }

    [System.Serializable]
    public class PduWriterConfig
    {
        public string name;
        public string class_name;
        public string conv_class_name;
        public string path;
        public string conv_path;
        public string pdu_config_name;
        public string topic_message_name;
    }

    [System.Serializable]
    public class PduReaderConfig
    {
        public string name;
        public string class_name;
        public string conv_class_name;
        public string path;
        public string conv_path;
        public string pdu_config_name;
        public string topic_message_name;
    }

    [System.Serializable]
    public class UdpMethodConfig
    {
        public string method_name;
        public string ipaddr;
        public int portno;
        public int iosize;
        public bool is_read;
    }
    [System.Serializable]
    public class MmapMethodConfig
    {
        public string method_name;
        public string filepath;
        public int iosize;
        public bool is_read;
    }
    [System.Serializable]
    public class ShmMethodConfig
    {
        public string method_name;
        public string asset_name;
        public int channel_id;
        public int iosize;
        public bool is_read;
    }
    [System.Serializable]
    public class RosTopicMethodConfig
    {
        public string name;
        public string class_name;
        public string path;
        public string parameters;
    }
    [System.Serializable]
    public class RostopicPublisherOption
    {
        public int cycle_scale;
        public bool latch;
        public int queue_size;
    }
    [System.Serializable]
    public class RosTopicMessageConfig
    {
        public string topic_message_name;
        public string topic_type_name;
        public string robot_name;
        public bool sub;
        public RostopicPublisherOption pub_option;
    }
    [System.Serializable]
    public class PduDataConfig
    {
        public string pdu_type_name;
        public PduDataFieldConfig[] fields;
        public string pdu_data_field_path;
    }
    [System.Serializable]
    public class PduDataFieldConfig
    {
        public string name;
        public string type;
    }
    [System.Serializable]
    public class ReaderConnectorConfig
    {
        public string name;
        public string pdu_name;
        public string method_name;
    }
    [System.Serializable]
    public class WriterConnectorConfig
    {
        public string name;
        public string pdu_name;
        public string method_name;
    }

    [System.Serializable]
    public class PduChannelConnectorConfig
    {
        public string outside_asset_name;
        public string reader_connector_name;
        public string writer_connector_name;
    }

    [System.Serializable]
    public class PduDataFieldsConfig
    {
        public PduDataFieldConfig[] fields;
    }
    [System.Serializable]
    public class RosTopicMessageConfigContainer
    {
        public RosTopicMessageConfig[] fields;
    }
    [System.Serializable]
    public class ParamWorldConfigContainer
    {
        public ParamScale scale;
    }
    [System.Serializable]
    public class ParamScale
    {
        public float scan;
        public float odom;
        public float cmdvel;
    }
    [System.Serializable]
    public class LoginRobotInfoType
    {
        public string roboname;
        public string robotype;
        public Vector3 pos;
        public Vector3 angle;
    }
    [System.Serializable]
    public class LoginRobot
    {
        public LoginRobotInfoType [] robos;
    }
    [System.Serializable]
    public class RobotTypes
    {
        public string [] robotype;
    }
}
