using Hakoniwa.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation.Logger
{
    public class SimulationLogger
    {
        private string filepath = null;
        private SimulationTimeStorage sim_time_logger = null;
        public SimulationLogger()
        {
        }
        public void SetFilePath(string path)
        {
            this.filepath = path;
        }

        public void Flush()
        {
            if (filepath == null)
            {
                return;
            }
            if (this.sim_time_logger != null)
            {
                this.sim_time_logger.Flush(filepath);
            }
        }

        public void SetColumnNames(string[] names)
        {
            this.sim_time_logger = new SimulationTimeStorage(names.Length);
            this.sim_time_logger.SetAssetNames(names);
        }

        public SimulationTimeStorage GetSimTimeLogger()
        {
            return this.sim_time_logger;
        }
    }
}
