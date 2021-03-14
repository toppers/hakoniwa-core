using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation.Environment
{
    public class SimulationEnvironment
    {
        private IEnvironmentOperation env;

        public SimulationEnvironment()
        {

        }

        public void Register(IEnvironmentOperation env_op)
        {
            this.env = env_op;
        }
        public void Save()
        {
            this.env.Save();
        }
        public void Restore()
        {
            this.env.Restore();
        }

    }
}
