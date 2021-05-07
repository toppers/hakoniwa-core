using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using Hakoniwa.PluggableAsset.Communication.Pdu.Ev3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets.Robot.EV3
{
    class Ev3ProtobufConverter : IInsideAssetController
    {
        private PduIoConnector pdu_io;
        private IPduReader in_pdu_reader;
        private IPduWriter in_pdu_writer;
        private Ev3PduProtobufReader out_pdu_reader;
        private Ev3PduProtobufWriter out_pdu_writer;

        private string name;

        public Ev3ProtobufConverter(string name)
        {
            this.name = name;
        }

        public void Initialize()
        {
            this.pdu_io = PduIoConnector.Get(this.GetName());
            this.in_pdu_reader = this.pdu_io.GetReader("Ev3ActuatorPdu");
            if (this.in_pdu_reader != null)
            {
                IPduReader tmp1 = this.pdu_io.GetReader("Ev3ActuatorProtobufPdu");
                if (tmp1 is Ev3PduProtobufReader)
                {
                    this.out_pdu_reader = (Ev3PduProtobufReader)tmp1;
                }
                else
                {
                    throw new ArgumentException(this.name + " can not cast class type: IPduReader -> Ev3PduProtobufReader");
                }
            }
            this.in_pdu_writer = this.pdu_io.GetWriter("Ev3SensorPdu");
            if (this.in_pdu_writer != null)
            {
                IPduWriter tmp2 = this.pdu_io.GetWriter("Ev3SensorProtobufPdu");
                if (tmp2 is Ev3PduProtobufWriter)
                {
                    this.out_pdu_writer = (Ev3PduProtobufWriter)tmp2;
                }
                else
                {
                    throw new ArgumentException(this.name + " can not cast class type: IPduWriter -> Ev3PduProtobufWriter");
                }
            }

        }
        public string GetName()
        {
            return name;
        }
        public void DoActuation()
        {
            if (this.in_pdu_reader != null)
            {
                var buf = Ev3PduProtobufReader.ProtoBufConvert(this.in_pdu_reader);
                this.out_pdu_reader.SetProtobuf(buf);
            }
        }
        public void CopySensingDataToPdu()
        {
            if (this.in_pdu_writer != null)
            {
                var buf = this.in_pdu_writer.GetDataBytes(null);
                this.out_pdu_writer.SetBuffer(buf);
            }
        }
    }
}
