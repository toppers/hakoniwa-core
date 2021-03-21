using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset
{
    [System.Serializable]
    public class CoreConfig
    {
        public string core_ipaddr;
        public int core_portno;
        public string SymTimeMeasureFilePath;
        public InsideAssetConfig []  inside_assets;
        public OutsideAssetConfig [] outside_assets;
        public PduWriterConfig[] pdu_writers;
        public PduReaderConfig[] pdu_readers;
        public UdpMethodConfig[] udp_methods;
        public MmapMethodConfig[] mmap_methods;
        public ReaderConnectorConfig[] reader_connectors;
        public WriterConnectorConfig[] writer_connectors;
        public PduChannelConnectorConfig[] pdu_channel_connectors;
    }
    [System.Serializable]
    public class InsideAssetConfig
    {
        public string name;
        public string [] pdu_writer_names;
        public string [] pdu_reader_names;
    }

    [System.Serializable]
    public class OutsideAssetConfig
    {
        public string name;
        public string class_name;
    }

    [System.Serializable]
    public class PduWriterConfig
    {
        public string name;
        public string class_name;
    }

    [System.Serializable]
    public class PduReaderConfig
    {
        public string name;
        public string class_name;
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

}
