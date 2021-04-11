using Hakoniwa.Core.Rpc;
using Hakoniwa.Core.Simulation;
using Hakoniwa.Core.Simulation.Environment;
using Hakoniwa.Core.Utils;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HakoniwaCoreTest
{
    class WorldController : IInsideWorldSimulatior, IEnvironmentOperation
    {
        public long deltaTime = 10 * 1000; /* usec */
        public long maxDelayTime = 20* 1000; /* usec */
        private SimulationController simulator = SimulationController.Get();

        public WorldController(string filepath)
        {
            AssetConfigLoader.Load(filepath);
            string ipaddr = AssetConfigLoader.core_config.core_ipaddr;
            int portno = AssetConfigLoader.core_config.core_portno;

            Console.WriteLine("ipaddr=" + ipaddr + " portno=" + portno.ToString());
            RpcServer.StartServer(ipaddr, portno);

            simulator.RegisterEnvironmentOperation(this);
            simulator.SaveEnvironment();
            simulator.GetLogger().SetFilePath(AssetConfigLoader.core_config.SymTimeMeasureFilePath);

            simulator.SetSimulationWorldTime(this.maxDelayTime, this.deltaTime); /* 10msec */
            simulator.SetInsideWorldSimulator(this);
        }
        public void Execute()
        {
            while (true)
            {
                try
                {
                    bool ret = this.simulator.Execute();
                    if (ret == false)
                    {
                        Thread.Sleep(1);
                    }
                }
                catch (Exception e)
                {
                    SimpleLogger.Get().Log(Level.ERROR, e);
                    throw e;
                }
            }
        }

        public void DoSimulation()
        {
            Thread.Sleep(1);
        }

        public void Restore()
        {
            return;
        }

        public void Save()
        {
            return;
        }
    }
}
