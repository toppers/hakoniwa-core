using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation.Environment
{
    public interface IEnvironmentOperation
    {
        void Save();
        void Restore();
    }
}
