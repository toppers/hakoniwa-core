using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Simulation.Environment;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset;
using Hakoniwa.PluggableAsset.Assets;
using Hakoniwa.PluggableAsset.Communication.Connector;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static Hakoniwa.Core.HakoCppWrapper;

namespace Hakoniwa.Core.Simulation
{
    class HakoAssetSimulationController : ISimulationController
    {
        private static HakoAssetSimulationController simulator = new HakoAssetSimulationController();
        private List<IInsideAssetController> inside_asset_list = null;

        internal static ISimulationController Get(string asset_name)
        {
            simulator.SetAssetName(asset_name);
            return simulator;
        }


        private HakoAssetSimulationController()
        {
            asset_manager = new SimulationAssetManager();
            sim_env = new SimulationEnvironment();
            HakoCppWrapper.asset_init();
        }
        private ISimulationAssetManager asset_manager;
        private SimulationEnvironment sim_env;
        private IInsideWorldSimulatior inside_simulator = null;
        private long asset_time_usec = 0;

        private string my_asset_name;
        private void SetAssetName(string asset_name)
        {
            my_asset_name = asset_name;
        }

        public void RegisterEnvironmentOperation(IEnvironmentOperation env_op)
        {
            sim_env.Register(env_op);
        }

        public void RestoreEnvironment()
        {
            this.asset_time_usec = 0;
            sim_env.Restore();
        }

        public void SaveEnvironment()
        {
            sim_env.Save();
        }

        public void SetInsideWorldSimulator(IInsideWorldSimulatior isim)
        {
            bool ret = HakoCppWrapper.asset_register_polling(my_asset_name);
            if (ret != true) {
                SimpleLogger.Get().Log(Level.ERROR, "SetInsideWorldSimulator:can not register asset");
            }
            this.inside_simulator = isim;
        }
        public long GetWorldTime()
        {
            return HakoCppWrapper.get_wrold_time();
        }
        private void PollEvent()
        {
            HakoSimAssetEvent ev = HakoCppWrapper.asset_get_event(this.my_asset_name);
            switch (ev) {
                case HakoSimAssetEvent.HakoSimAssetEvent_Start:
                    StartCallback();
                    break;
                case HakoSimAssetEvent.HakoSimAssetEvent_Stop:
                    StopCallback();
                    break;
                case HakoSimAssetEvent.HakoSimAssetEvent_Reset:
                    ResetCallback();
                    break;
                default:
                    break;
            }
        }
        public void ReadPdu()
        {
            /********************
             * Inside assets
             * - Recv Actuation Data
            ********************/
            foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
            {
                if (
                    ((connector.GetName() == null) || connector.GetName().Equals("None"))
                        && (connector.Reader != null)
                    )
                {
                    //TODO pdu read
                    connector.Reader.Recv();
                }
            }

            HakoCppWrapper.asset_notify_read_pdu_done(my_asset_name);
        }

        public void WritePdu()
        {
            /********************
             * Onside assets 
             * - Send Sensor Data 
             ********************/
            foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
            {
                if (
                    ((connector.GetName() == null) || connector.GetName().Equals("None"))
                        && (connector.Writer != null))
                {
                    //TODO write pdu
                    connector.Writer.SendWriterPdu();
                    connector.Writer.SendReaderPdu();
                }
            }
            HakoCppWrapper.asset_notify_write_pdu_done(my_asset_name);     
        }

        public void ExecuteSimulation()
        {
            /********************
             * Inside Assets 
             * - Do Simulation
             ********************/
            foreach (var asset in inside_asset_list)
            {
                asset.DoActuation();
            }

            this.inside_simulator.DoSimulation();

            foreach (var asset in inside_asset_list)
            {
                asset.CopySensingDataToPdu();
            }
        }
        public bool Execute()
        {
            //for heartbeat 
            HakoCppWrapper.asset_notify_simtime(my_asset_name, this.asset_time_usec);
            this.PollEvent();

            if (this.GetState() != SimulationState.Running) {
                return false;
            }

            /********************
             * Hakoniwa Time Sync
             ********************/
            if (HakoCppWrapper.asset_is_pdu_created() == false)
            {
                /* nothing to do */
                SimpleLogger.Get().Log(Level.INFO, "Execute:pdu is not created");
                return false;
            }
            else if (HakoCppWrapper.asset_is_simulation_mode())
            {
                long world_time = this.GetWorldTime();
                if (this.asset_time_usec < world_time)
                {
                    this.asset_time_usec += this.inside_simulator.GetDeltaTimeUsec();
                    HakoCppWrapper.asset_notify_simtime(my_asset_name, this.asset_time_usec);
                }
                else {
                    // can not do simulation because world time is slow...
                    //SimpleLogger.Get().Log(Level.INFO, "Execute:skip.. asset_time={0} world_time={1}", this.asset_time_usec, world_time);
                    return false;
                }
                this.ReadPdu();
                this.ExecuteSimulation();
                this.WritePdu();
                return true;
            }
            else if (HakoCppWrapper.asset_is_pdu_sync_mode(my_asset_name))
            {
                this.WritePdu();
                //SimpleLogger.Get().Log(Level.INFO, "Execute:skip.. WritePdu()");
                return false;
            }
            //SimpleLogger.Get().Log(Level.INFO, "Execute:skip.. why??");
            return false;
        }

        private void StartCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "StartCallback");
            inside_asset_list = asset_manager.RefInsideAssetList();
            HakoCppWrapper.asset_start_feedback(my_asset_name, true);
        }
        private void StopCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "StopCallback");
            HakoCppWrapper.asset_stop_feedback(my_asset_name, true);
        }
        private void ResetCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "ResetCallback");
            PduIoConnector.Reset();
            this.RestoreEnvironment();
            foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
            {
                if (connector.Reader != null)
                {
                    connector.Reader.Reset();
                }
            }
            HakoCppWrapper.asset_reset_feedback(my_asset_name, true);
        }

        public SimulationState GetState()
        {
            HakoState state = HakoCppWrapper.simevent_get_state();
            switch (state)
            {
                case HakoState.Stopped:
                case HakoState.Resetting:
                    return SimulationState.Stopped;
                case HakoState.Runnable:
                    return SimulationState.Runnable;
                case HakoState.Running:
                    return SimulationState.Running;
                case HakoState.Stopping:
                    return SimulationState.Stopping;
                case HakoState.Error:
                case HakoState.Terminated:
                default:
                    return SimulationState.Terminated;
            }
        }

        public bool Start()
        {
            return HakoCppWrapper.simevent_start();
        }

        public bool Stop()
        {
            return HakoCppWrapper.simevent_stop();
        }

        public bool Reset()
        {
            return HakoCppWrapper.simevent_reset();
        }

        public ISimulationAssetManager GetAssetManager()
        {
            return asset_manager;
        }

        public int RefOutsideAssetListCount()
        {
            return 0;
        }
    }
}
