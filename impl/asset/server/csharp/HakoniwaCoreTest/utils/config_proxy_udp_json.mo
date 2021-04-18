{
    "core_ipaddr": "{{RESOLVE_IPADDR}}",
    "core_portno": 50051,
    "asset_timeout": 3,
        "SymTimeMeasureFilePath": ".\\core.csv",
        "inside_assets": null,
    "outside_assets": [
        {
	        	"name": "Athrill_1",
	        	"class_name": "Ev3MiconAssetController",
        	},
	       	{
	        	"name": "Athrill_2",
	        	"class_name": "Ev3MiconAssetController",
	        },
	       	{
	        	"name": "Athrill_3",
	        	"class_name": "Ev3MiconAssetController",
	        },
	       	{
	        	"name": "Athrill_4",
	        	"class_name": "Ev3MiconAssetController",
	        }
    ],
    "pdu_writers": [
        {
	        	"name": "MiconSensorPdu-1",
	        	"class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Micon.SimpleMiconPduWriter",
	        	"path": "..\\..\\..\\..\\AssetPdu\\MiconPdu\\bin\\Debug\\netstandard2.0\\MiconPdu.dll",
        	},
	       	{
	        	"name": "MiconSensorPdu-2",
	        	"class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Micon.SimpleMiconPduWriter",
	        	"path": "..\\..\\..\\..\\AssetPdu\\MiconPdu\\bin\\Debug\\netstandard2.0\\MiconPdu.dll",
        	},
	       	{
	        	"name": "MiconSensorPdu-3",
	        	"class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Micon.SimpleMiconPduWriter",
	        	"path": "..\\..\\..\\..\\AssetPdu\\MiconPdu\\bin\\Debug\\netstandard2.0\\MiconPdu.dll",
        	},
	       	{
	        	"name": "MiconSensorPdu-4",
	        	"class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Micon.SimpleMiconPduWriter",
	        	"path": "..\\..\\..\\..\\AssetPdu\\MiconPdu\\bin\\Debug\\netstandard2.0\\MiconPdu.dll",
        	}
    ],
    "pdu_readers": [
        {
	        	"name": "MiconActuatorPdu-1",
	        	"class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Micon.SimpleMiconPduReader",
	        	"path": "..\\..\\..\\..\\AssetPdu\\MiconPdu\\bin\\Debug\\netstandard2.0\\MiconPdu.dll",
        	},
	       	{
	        	"name": "MiconActuatorPdu-2",
	        	"class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Micon.SimpleMiconPduReader",
	        	"path": "..\\..\\..\\..\\AssetPdu\\MiconPdu\\bin\\Debug\\netstandard2.0\\MiconPdu.dll",
	        },
	       	{
	        	"name": "MiconActuatorPdu-3",
	        	"class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Micon.SimpleMiconPduReader",
	        	"path": "..\\..\\..\\..\\AssetPdu\\MiconPdu\\bin\\Debug\\netstandard2.0\\MiconPdu.dll",
	        },
	       	{
	        	"name": "MiconActuatorPdu-4",
	        	"class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Micon.SimpleMiconPduReader",
	        	"path": "..\\..\\..\\..\\AssetPdu\\MiconPdu\\bin\\Debug\\netstandard2.0\\MiconPdu.dll",
	        }
    ],
    "udp_methods": [
        {
            "method_name": "UdpMethod_write1",
	        	"ipaddr": "{{IFCONFIG_IPADDR}}",
        	    "portno": 54002,
        	    "iosize": 1024,
	        	"is_read": false,
        },
        {
            "method_name": "UdpMethod_write2",
	        	"ipaddr": "{{IFCONFIG_IPADDR}}",
	        	"portno": 54012,
        	    "iosize": 1024,
	        	"is_read": false,
        },
        {
            "method_name": "UdpMethod_write3",
	        	"ipaddr": "{{IFCONFIG_IPADDR}}",
	        	"portno": 54022,
        	    "iosize": 1024,
	        	"is_read": false,
        },
        {
            "method_name": "UdpMethod_write4",
	        	"ipaddr": "{{IFCONFIG_IPADDR}}",
	        	"portno": 54032,
        	    "iosize": 1024,
	        	"is_read": false,
        },
        {
            "method_name": "UdpMethod_read1",
	        	"ipaddr": "{{RESOLVE_IPADDR}}",
	        	"portno": 54001,
        	    "iosize": 1024,
	        	"is_read": true,
        },
        {
	        	"method_name": "UdpMethod_read2",
	        	"ipaddr": "{{RESOLVE_IPADDR}}",
	        	"portno": 54011,
    	        "iosize": 1024,
	        	"is_read": true,
       	},
        {
	        	"method_name": "UdpMethod_read3",
	        	"ipaddr": "{{RESOLVE_IPADDR}}",
	        	"portno": 54021,
	            "iosize": 1024,
	        	"is_read": true,
       	},
        {
	        	"method_name": "UdpMethod_read4",
	        	"ipaddr": "{{RESOLVE_IPADDR}}",
	        	"portno": 54031,
	            "iosize": 1024,
	        	"is_read": true,
       	},
    ],
    "mmap_methods": null,
    "reader_connectors":  [
	       	{
	        	"name": "reader_connector1",
	        	"pdu_name": "MiconActuatorPdu-1",
	        	"method_name": "UdpMethod_read1",
        	},
	       	{
	        	"name": "reader_connector2",
	        	"pdu_name": "MiconActuatorPdu-2",
	        	"method_name": "UdpMethod_read2",
        	},
	       	{
	        	"name": "reader_connector3",
	        	"pdu_name": "MiconActuatorPdu-3",
	        	"method_name": "UdpMethod_read3",
        	},
	       	{
	        	"name": "reader_connector4",
	        	"pdu_name": "MiconActuatorPdu-4",
	        	"method_name": "UdpMethod_read4",
        	}
        ],
    "writer_connectors":  [
	       	{
	        	"name": "writer_connector1",
	        	"pdu_name": "MiconSensorPdu-1",
	        	"method_name": "UdpMethod_write1",
        	},
	       	{
	        	"name": "writer_connector2",
	        	"pdu_name": "MiconSensorPdu-2",
	        	"method_name": "UdpMethod_write2",
        	},
	       	{
	        	"name": "writer_connector3",
	        	"pdu_name": "MiconSensorPdu-3",
	        	"method_name": "UdpMethod_write3",
        	},
	       	{
	        	"name": "writer_connector4",
	        	"pdu_name": "MiconSensorPdu-4",
	        	"method_name": "UdpMethod_write4",
        	},
    ],    
    "pdu_channel_connectors":  [
	       	{
	        	"outside_asset_name": "Athrill_1",
	        	"reader_connector_name": "reader_connector1",
	        	"writer_connector_name": "writer_connector1",
        	},
	       	{
	        	"outside_asset_name": "Athrill_2",
	        	"reader_connector_name": "reader_connector2",
	        	"writer_connector_name": "writer_connector2",
        	},
	       	{
	        	"outside_asset_name": "Athrill_3",
	        	"reader_connector_name": "reader_connector3",
	        	"writer_connector_name": "writer_connector3",
        	},
	       	{
	        	"outside_asset_name": "Athrill_4",
	        	"reader_connector_name": "reader_connector4",
	        	"writer_connector_name": "writer_connector4",
        	}
        ],
}