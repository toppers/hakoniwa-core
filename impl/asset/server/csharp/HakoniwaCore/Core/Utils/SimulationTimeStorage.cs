using Hakoniwa.Core.Utils.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.Core.Utils
{
    public class SimulationTimeStorage
    {
        private LineBinaryStorage storage;
        private string [] asset_names = null;

        public SimulationTimeStorage(int asset_num)
        {
            this.storage = new LineBinaryStorage(asset_num, 8);
        }
        public void SetAssetNames(string [] names)
        {
            if (names.Length != this.storage.GetColumns())
            {
                return;
            }
            this.asset_names = names;
        }

        public void SetSimTime(int asset_id, UInt64 simtime)
        {
            this.storage.SetData(asset_id, simtime);
        }
        public void SetSimTime(int asset_id, double simtime)
        {
            this.storage.SetData(asset_id, simtime);
        }
        public void Next()
        {
            this.storage.Next();
        }

        public void Flush(string filepath)
        {
            try
            {
                StreamWriter writer = new StreamWriter(filepath, false);
                string headers = null;
                if (this.asset_names != null)
                {
                    for (int column = 0; column < this.storage.GetColumns(); column++)
                    {
                        if (headers == null)
                        {
                            headers = this.asset_names[column] + ",";
                        }
                        else
                        {
                            headers = headers + this.asset_names[column] + ",";
                        }
                    }
                    writer.WriteLine(headers);
                }

                int lines = this.storage.GetLines();
                for (int i = 0; i < lines; i++)
                {
                    string message = null;
                    for (int column = 0; column < this.storage.GetColumns(); column++)
                    {
                        double value;
                        if (this.storage.GetData(i, column, out value))
                        {
                            if (message == null)
                            {
                                message = value.ToString() + ",";
                            }
                            else
                            {
                                message = message + value.ToString() + ",";
                            }
                        }
                        else
                        {
                            if (message == null)
                            {
                                message = "INVALID" + ",";
                            }
                            else
                            {
                                message = message + "INVALID" + ",";
                            }
                        }
                    }
                    writer.WriteLine(message);
                }
                writer.Flush();
                writer.Close();
                writer.Dispose();
                writer = null;
            }
            catch (Exception e)
            {
                SimpleLogger.Get().Log(Level.ERROR, e);
                throw e;
            }
        }
    }
}
