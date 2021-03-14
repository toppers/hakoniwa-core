using System;
using System.Collections.Generic;
using System.Text;
using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Simulation.Environment;
using Hakoniwa.Core.Simulation.Logger;

namespace Hakoniwa.Core.Simulation
{
    public enum SimulationState
    {
        Stopped = 0,
        Runnable = 1,
        Running = 2,
        Stopping = 3,
        Terminated = 99,
    }
    public enum HakoniwaSimulationResult
    {
        Success = 0,
        Failed = 1,
    }
    public class SimulationController
    {
        private static SimulationController simulator = new SimulationController();
        private Object lockObj = new Object();
        private SimulationLogger logger = new SimulationLogger();
        public SimulationState state = SimulationState.Stopped;

        public static SimulationController Get()
        {
            return simulator;
        }

        public HakoniwaSimulationResult result = HakoniwaSimulationResult.Success;

        private int asset_feedback_count;
        public AssetManager asset_mgr = new AssetManager();
        public RequestManager req_mgr = new RequestManager();

        private SimulationEnvironment sim_env;

        public SimulationLogger GetLogger()
        {
            return this.logger;
        }
        public SimulationController()
        {
            this.sim_env = new SimulationEnvironment();
        }
        public void RegisterEnvironmentOperation(IEnvironmentOperation env_op)
        {
            this.sim_env.Register(env_op);
        }
        public void SaveEnvironment()
        {
            this.sim_env.Save();
        }
        public void RestoreEnvironment()
        {
            this.sim_env.Restore();
        }
        private bool AssetFeedback(bool isOK)
        {
            bool all_done = false;
            Console.WriteLine("AssetFeedback:" + isOK);
            this.asset_feedback_count++;
            if (isOK == false)
            {
                this.result = HakoniwaSimulationResult.Failed;
            }
            all_done = (this.asset_feedback_count == this.asset_mgr.GetAssetCount());
            return all_done;
        }
        private void PublishEvent(CoreAssetNotificationEvent ev)
        {
            this.asset_feedback_count = 0;
            this.result = HakoniwaSimulationResult.Success;
            req_mgr.PutEvent(ev);
            foreach (var asset in asset_mgr.GetAssetList())
            {
                asset_mgr.SetEvent(asset.GetName(), new AssetEvent(ev));
            }
        }

        public bool Start()
        {
            lock (this.lockObj)
            {
                if (state == SimulationState.Stopped)
                {
                    state = SimulationState.Runnable;
                    PublishEvent(CoreAssetNotificationEvent.Start);
                    Console.WriteLine("StateChanged:" + state);
                    return true;
                }
                else
                {
                    Console.WriteLine("StateNotChanged:" + state);
                    return false;
                }
            }
        }
        public bool StartFeedback(bool isStarted)
        {
            lock (this.lockObj)
            {
                Console.WriteLine("StartFeedback:" + state);
                if (state == SimulationState.Runnable)
                {
                    if (AssetFeedback(isStarted))
                    {
                        Console.WriteLine("StateChanged:" + state);
                        state = SimulationState.Running;

                        string[] names = new string[this.asset_mgr.GetAssetCount() + 1];
                        names[0] = "Hakoniwa";
                        int i = 1;
                        foreach (var asset in this.asset_mgr.GetAssetList())
                        {
                            names[i] = asset.GetName();
                            i++;
                        }
                        this.logger.SetColumnNames(names);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    AssetFeedback(false);
                    Console.WriteLine("StateNotChanged:" + state);
                    return false;
                }
            }
        }
        public bool Stop()
        {
            lock (this.lockObj)
            {
                if (state == SimulationState.Running)
                {
                    state = SimulationState.Stopping;
                    PublishEvent(CoreAssetNotificationEvent.Stop);
                    Console.WriteLine("StateChanged:" + state);
                    return true;
                }
                else
                {
                    Console.WriteLine("StateNotChanged:" + state);
                    return false;
                }
            }

        }
        public bool StopFeedback(bool isStopped)
        {
            lock (this.lockObj)
            {
                if (state == SimulationState.Stopping)
                {
                    if (AssetFeedback(isStopped))
                    {
                        state = SimulationState.Stopped;
                        Console.WriteLine("StateChanged:" + state);
                        this.logger.Flush();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    AssetFeedback(false);
                    Console.WriteLine("StateNotChanged:" + state);
                    return false;
                }
            }
        }
        public bool Terminate()
        {
            lock (this.lockObj)
            {
                if (state == SimulationState.Stopped)
                {
                    state = SimulationState.Terminated;
                    PublishEvent(CoreAssetNotificationEvent.None);
                    Console.WriteLine("StateChanged:" + state);
                    return true;
                }
                else
                {
                    Console.WriteLine("StateNotChanged:" + state);
                    return false;
                }
            }
        }

        public SimulationState GetState()
        {
            return state;
        }
    }
}
