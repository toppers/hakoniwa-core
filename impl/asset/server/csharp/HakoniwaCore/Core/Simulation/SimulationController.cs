using System;
using System.Collections.Generic;
using System.Text;
using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Simulation.Environment;
using Hakoniwa.Core.Simulation.Logger;
using Hakoniwa.Core.Simulation.Time;
using Hakoniwa.Core.Utils;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset;
using Hakoniwa.PluggableAsset.Assets;
using Hakoniwa.PluggableAsset.Communication.Connector;

namespace Hakoniwa.Core.Simulation
{
    public class SimulationController : ISimulationController, ISimulationAssetManager
    {
        private static SimulationController simulator = new SimulationController();
        private System.Object lockObj = new System.Object();
        private IInsideWorldSimulatior inside_simulator = null;
        private SimulationLogger logger = new SimulationLogger();
        public SimulationState state = SimulationState.Stopped;
        private TheWorld theWorld = new TheWorld();

        public static SimulationController Get()
        {
            return simulator;
        }

        public HakoniwaSimulationResult result = HakoniwaSimulationResult.Success;

        private AssetManager asset_mgr = new AssetManager();
        List<IOutsideAssetController> outside_asset_list = null;
        List<IInsideAssetController> inside_asset_list = null;

        private SimulationEnvironment sim_env;

        public SimulationLogger GetLogger()
        {
            return this.logger;
        }
        private SimulationController()
        {
            this.sim_env = new SimulationEnvironment();
            this.theWorld.SetMaxDelayTime(20000); //TODO
            this.theWorld.SetDeltaTime(1); //TODO
        }

        public bool RegisterOutsideAsset(string name)
        {
            lock (this.lockObj)
            {
                bool ret = false;
                try
                {
                    ret = this.asset_mgr.RegisterOutsideAsset(name);
                }
                catch (Exception e)
                {
                    SimpleLogger.Get().Log(Level.ERROR, "RegisterOutsideAsset error:" + name);
                    SimpleLogger.Get().Log(Level.TRACE, e);
                }
                return ret;
            }
        }
        public bool RegisterInsideAsset(string name)
        {
            lock (this.lockObj)
            {
                return this.asset_mgr.RegisterInsideAsset(name);
            }
        }
        public int RefOutsideAssetListCount()
        {
            lock (this.lockObj)
            {
                return this.asset_mgr.RefOutsideAssetList().Count;
            }
        }

        public void SetInsideWorldSimulator(IInsideWorldSimulatior isim)
        {
#if false
            if (AssetConfigLoader.core_config.inside_assets != null)
            {
                foreach (var iasset in AssetConfigLoader.core_config.inside_assets)
                {
                    if (iasset.core_class_name != null)
                    {
                        var controller = AssetConfigLoader.GetInsideAsset(iasset.name);
                        controller.Initialize();
                        this.RegisterInsideAsset(iasset.name);
                    }
                }
            }
#endif
            this.inside_simulator = isim;
        }
        public void SetSimulationWorldTime(long max_delay_time, long delta_time)
        {
            this.theWorld.SetMaxDelayTime(max_delay_time);
            this.theWorld.SetDeltaTime(delta_time);
        }
        public void RegisterEnvironmentOperation(IEnvironmentOperation env_op)
        {
            this.sim_env.Register(env_op);
        }
        public void SaveEnvironment()
        {
            this.theWorld.Save();
            this.sim_env.Save();
        }
        public void RestoreEnvironment()
        {
            this.theWorld.Restore();
            this.sim_env.Restore();
        }

        public void Unregister(string name)
        {
            lock (this.lockObj)
            {
                try
                {
                    this.asset_mgr.Unregister(name);
                }
                catch (Exception e)
                {
                    SimpleLogger.Get().Log(Level.ERROR, "Unregister error:" + name);
                    SimpleLogger.Get().Log(Level.TRACE, e);
                }
            }
        }

        private bool reset_request = false;
        public bool ResetRequest()
        {
            lock (this.lockObj)
            {
                if (state == SimulationState.Stopped)
                {
                    this.reset_request = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool Reset()
        {
            lock (this.lockObj)
            {
                if (state == SimulationState.Stopped)
                {
                    PduIoConnector.Reset();
                    this.RestoreEnvironment();
                    foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
                    {
                        if (connector.Reader != null)
                        {
                            connector.Reader.Reset();
                        }
                    }
                    this.reset_request = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private bool AssetFeedback(string name, bool isOK)
        {
            bool all_done = false;
            SimpleLogger.Get().Log(Level.INFO, "AssetFeedback("+ name +"):" + isOK);
            string found_asset = null;
            foreach (var asset in this.event_list)
            {
                if (asset.Equals(name))
                {
                    found_asset = asset;
                    break;
                }
            }
            if (found_asset != null)
            {
                this.event_list.Remove(found_asset);
                if (isOK == false)
                {
                    this.result = HakoniwaSimulationResult.Failed;
                }
            }
            else
            {
                this.result = HakoniwaSimulationResult.Failed;
            }
            all_done = (this.event_list.Count == 0);
            return all_done;
        }
        private List<string> event_list;
        private void PublishEvent(CoreAssetNotificationEvent ev)
        {
            this.event_list = new List<string>();
            this.result = HakoniwaSimulationResult.Success;
            //req_mgr.PutEvent(ev); TODO
            foreach (var asset in asset_mgr.RefOutsideAssetList())
            {
                event_list.Add(asset.GetName());
                asset_mgr.SetEvent(asset.GetName(), new AssetEvent(ev));
            }
            foreach (var asset in asset_mgr.RefInsideAssetList())
            {
                event_list.Add(asset.GetName());
            }
        }
        private void StartLogging()
        {
            string[] names = new string[this.asset_mgr.RefOutsideAssetList().Count + 3];
            names[0] = "Host";
            names[1] = "Hakoniwa-prev";
            int i = 2;
            foreach (var asset in this.asset_mgr.RefOutsideAssetList())
            {
                names[i] = asset.GetName();
                i++;
            }
            names[i] = "Hakoniwa-after";
            this.logger.SetColumnNames(names);
        }
        public bool Start()
        {
            lock (this.lockObj)
            {
                if (state == SimulationState.Stopped)
                {
                    state = SimulationState.Runnable;
                    PublishEvent(CoreAssetNotificationEvent.Start);
                    foreach (var asset in asset_mgr.RefInsideAssetList())
                    {
                        this.StartFeedback(asset.GetName(), true);
                    }

                    SimpleLogger.Get().Log(Level.INFO, "StateChanged:" + state);
                    return true;
                }
                else
                {
                    SimpleLogger.Get().Log(Level.INFO, "StateNotChanged:" + state);
                    return false;
                }
            }
        }

        public bool IsExist(string name)
        {
            lock (this.lockObj)
            {
                return this.asset_mgr.IsExist(name);
            }
        }

        public bool StartFeedback(string name, bool isStarted)
        {
            lock (this.lockObj)
            {
                SimpleLogger.Get().Log(Level.INFO, "StartFeedback:" + state);
                if (state == SimulationState.Runnable)
                {
                    if (AssetFeedback(name, isStarted))
                    {
                        SimpleLogger.Get().Log(Level.INFO, "StateChanged:" + state);
                        state = SimulationState.Running;
                        this.StartLogging();
                        this.outside_asset_list = this.asset_mgr.RefOutsideAssetList();
                        this.inside_asset_list = this.asset_mgr.RefInsideAssetList();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    AssetFeedback(name, false);
                    SimpleLogger.Get().Log(Level.INFO, "StateNotChanged:" + state);
                    return false;
                }
            }
        }

        public AssetEvent GetEvent(string name)
        {
            lock (this.lockObj)
            {
                return this.asset_mgr.GetEvent(name);
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
                    foreach (var asset in asset_mgr.RefInsideAssetList())
                    {
                        this.StopFeedback(asset.GetName(), true);
                    }
                    SimpleLogger.Get().Log(Level.INFO, "StateChanged:" + state);
                    return true;
                }
                else
                {
                    SimpleLogger.Get().Log(Level.INFO, "StateNotChanged:" + state);
                    return false;
                }
            }

        }
        public bool StopFeedback(string name, bool isStopped)
        {
            lock (this.lockObj)
            {
                if (state == SimulationState.Stopping)
                {
                    if (AssetFeedback(name, isStopped))
                    {
                        state = SimulationState.Stopped;
                        SimpleLogger.Get().Log(Level.INFO, "StateChanged:" + state);
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
                    AssetFeedback(name, false);
                    SimpleLogger.Get().Log(Level.INFO, "StateNotChanged:" + state);
                    return false;
                }
            }
        }

        public List<AssetEntry> GetAssetList()
        {
            lock (this.lockObj)
            {
                return this.asset_mgr.GetAssetList();
            }
        }

        public void UpdateTime(string name)
        {
            lock (this.lockObj)
            {
                this.asset_mgr.UpdateTime(name);
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
                    SimpleLogger.Get().Log(Level.INFO, "StateChanged:" + state);
                    return true;
                }
                else
                {
                    SimpleLogger.Get().Log(Level.INFO, "StateNotChanged:" + state);
                    return false;
                }
            }
        }

        public SimulationState GetState()
        {
            return state;
        }

        public long GetWorldTime()
        {
            return this.theWorld.GetWorldTime();
        }

        private void UnregisterDeadAssets()
        {
            if (AssetConfigLoader.core_config.asset_timeout < 0)
            {
                return;
            }
            lock (this.lockObj)
            {
                foreach (var asset in this.asset_mgr.RefOutsideAssetList())
                {
                    if (this.asset_mgr.IsTimeout(asset.GetName(), AssetConfigLoader.core_config.asset_timeout * 1000000))
                    {
                        if (this.state == SimulationState.Runnable)
                        {
                            this.StartFeedback(asset.GetName(), false);
                        }
                        else if (this.state == SimulationState.Stopping)
                        {
                            this.StopFeedback(asset.GetName(), false);
                        }
                        SimpleLogger.Get().Log(Level.ERROR, "OutSideAsset dead:" + asset.GetName());

                        this.asset_mgr.Unregister(asset.GetName());
                    }
                }
                this.outside_asset_list = this.asset_mgr.RefOutsideAssetList();
            }
        }
        public bool Execute()
        {
            if (state != SimulationState.Running)
            {
                if (this.reset_request)
                {
                    this.Reset();
                }
                this.UnregisterDeadAssets();
                return false;
            }
            long prev_hakoniwa_time = theWorld.GetWorldTime();
            /********************
             * Inside assets
             * - Recv Actuation Data
             ********************/
            foreach (var asset in this.outside_asset_list) 
            {
                asset.RecvPdu();
            }
            foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
            {
                if (
                    ((connector.GetName() == null) || connector.GetName().Equals("None"))
                        && (connector.Reader != null)
                    )
                {
                    connector.Reader.Recv();
                }
            }

            /********************
             * Hakoniwa Time Sync
             ********************/
            bool canStep = theWorld.CanStep(this.outside_asset_list);
            //SimpleLogger.Get().Log(Level.DEBUG, "canStep=" + canStep);
            if (canStep)
            {
                /********************
                 * Inside Assets 
                 * - Do Simulation
                 ********************/
                foreach (var asset in this.inside_asset_list)
                {
                    asset.DoActuation();
                }

                this.inside_simulator.DoSimulation();

                foreach (var asset in this.inside_asset_list)
                {
                    asset.CopySensingDataToPdu();
                }
                theWorld.StepFoward();
            }

            /********************
             * Onside assets 
             * - Send Sensor Data 
             * - Time Sync
             ********************/
            int i = 2;
            foreach (var asset in this.outside_asset_list)
            {
                this.logger.GetSimTimeLogger().SetSimTime(i++, ((double)asset.GetSimTime()) / 1000000f);
                asset.PutHakoniwaTime(theWorld.GetWorldTime());
                asset.SendPdu();
            }
            this.logger.GetSimTimeLogger().SetSimTime(0, UtilTime.GetUnixTime());
            this.logger.GetSimTimeLogger().SetSimTime(1, ((double)prev_hakoniwa_time) / 1000000f);
            this.logger.GetSimTimeLogger().SetSimTime(i, ((double)theWorld.GetWorldTime()) / 1000000f);
            this.logger.GetSimTimeLogger().Next();
            foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
            {
                if (
                    ((connector.GetName() == null) || connector.GetName().Equals("None"))
                        && (connector.Writer != null))
                {
                    connector.Writer.SendWriterPdu();
                    connector.Writer.SendReaderPdu();
                }
            }
            return canStep;
        }

        public ISimulationAssetManager GetAssetManager()
        {
            return this;
        }

        public List<IInsideAssetController> RefInsideAssetList()
        {
            throw new NotImplementedException();
        }
    }
}
