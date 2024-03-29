﻿using Hakoniwa.Core.Rpc;
using Hakoniwa.Core.Simulation;
using Hakoniwa.Core.Simulation.Environment;
using Hakoniwa.Core.Utils;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HakoniwaRunner
{
    class HakoniwaRunner : IInsideWorldSimulatior, IEnvironmentOperation
    {
        public long deltaTime = 10 * 1000; /* usec */
        public long maxDelayTime = 20 * 1000; /* usec */
        public long sim_exec_count_max = 10;
        private SimulationController simulator = SimulationController.Get();

        public HakoniwaRunner(string filepath, int cnt)
        {
            this.sim_exec_count_max = cnt;
            AssetConfigLoader.Load(filepath);
            string ipaddr = AssetConfigLoader.core_config.core_ipaddr;
            int portno = AssetConfigLoader.core_config.core_portno;

            if (AssetConfigLoader.core_config.sim_time_sync != null)
            {
                deltaTime = AssetConfigLoader.core_config.sim_time_sync.deltaTimeMsec * 1000;
                maxDelayTime = AssetConfigLoader.core_config.sim_time_sync.maxDelayTimeMsec * 1000;
            }
            Console.WriteLine("ipaddr=" + ipaddr + " portno=" + portno.ToString());
            Console.WriteLine("deltaTimeMsec=" + deltaTime/1000 + " maxDelayTimeMsec=" + maxDelayTime/1000);
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
                    long sim_exec_count = 0;
                    var sim_start_realtime = UtilTime.GetUnixTime();
                    while (sim_exec_count < sim_exec_count_max)
                    {
                        if (this.simulator.Execute())
                        {
                            sim_exec_count++;
                        }
                    }
                    long elaps_simtime = sim_exec_count * this.deltaTime;
                    var sim_end_realtime = UtilTime.GetUnixTime();
                    var elaps_realtime = sim_end_realtime - sim_start_realtime;
                    if (elaps_realtime < elaps_simtime)
                    {
                        //Console.WriteLine("sleep " + ((int)(interval - diff) / 1000) +" msec diff=" + diff);
                        Thread.Sleep((int)(elaps_simtime - elaps_realtime) / 1000);
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
            return;
        }

        public void Restore()
        {
            return;
        }

        public void Save()
        {
            return;
        }
        static void Main(string[] args)
        {
            string filepath = "./core_config.json";

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: HakoniwaRunner sim_unit_count");
                return;
            }

            var controller = new HakoniwaRunner(filepath, int.Parse(args[0]));

            controller.Execute();

            Console.WriteLine("Shutdown START");
            RpcServer.ShutdownServer();
            Console.WriteLine("Shutdown END");
        }
    }
}
