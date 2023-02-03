#if NO_USE_GRPC
#else

using Hakoniwa.Core.Rpc;
using Hakoniwa.Core.Simulation.Environment;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset;
using Hakoniwa.PluggableAsset.Assets;
using Hakoniwa.PluggableAsset.Communication.Connector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hakoniwa.Core.Simulation
{
    class HakoRpcAssetSimulationController : ISimulationController, RpcClientCallback
    {
        private static HakoRpcAssetSimulationController simulator = new HakoRpcAssetSimulationController();
        private List<IInsideAssetController> inside_asset_list = null;

        internal static ISimulationController Get(string asset_name)
        {
            simulator.SetAssetName(asset_name);
            return simulator;
        }

        private string my_asset_name;
        private void SetAssetName(string asset_name)
        {
            my_asset_name = asset_name;
        }

        private HakoRpcAssetSimulationController()
        {
            asset_manager = new SimulationAssetManager();
            sim_env = new SimulationEnvironment();
            this.attrs.state = SimulationState.Stopped;
            this.attrs.master_time = 0;
            this.attrs.is_simulation_mode = false;
            this.attrs.is_pdu_sync_mode = false;
            this.attrs.is_pdu_created = false;
        }

        private ISimulationAssetManager asset_manager;
        private SimulationEnvironment sim_env;
        private IInsideWorldSimulatior inside_simulator = null;
        private long asset_time_usec = 0;
        private SimulationAttrs attrs = new SimulationAttrs();
        private bool is_pdu_read_done = false;
        private bool is_pdu_write_done = false;

        public bool Execute()
        {
            //for heartbeat
            bool result = RpcClient.NotifySimtime(
                this.my_asset_name,
                this.asset_time_usec,
                this.is_pdu_read_done,
                this.is_pdu_write_done,
                ref this.attrs);
            if (result == false)
            {
                return false;
            }
            if (this.GetState() != SimulationState.Running)
            {
                //SimpleLogger.Get().Log(Level.INFO, "SKIP state is not Running: " + this.GetState());
                DoResetCallback();
                return false;
            }
            if (this.attrs.is_pdu_created == false)
            {
                /* nothing to do */
                SimpleLogger.Get().Log(Level.INFO, "Execute:pdu is not created");
                return false;
            }
            else if (this.attrs.is_simulation_mode)
            {
                long world_time = this.GetWorldTime();
                if (this.asset_time_usec < world_time)
                {
                    this.asset_time_usec += this.inside_simulator.GetDeltaTimeUsec();
                }
                else
                {
                    // can not do simulation because world time is slow...
                    SimpleLogger.Get().Log(Level.INFO, "Execute:skip.. asset_time={0} world_time={1}", this.asset_time_usec, world_time);
                    return false;
                }
                this.ReadPdu();
                this.ExecuteSimulation();
                this.WritePdu();
                return true;
            }
            else if (this.attrs.is_pdu_sync_mode)
            {
                this.WritePdu();
                SimpleLogger.Get().Log(Level.INFO, "Execute:skip.. WritePdu()");
                return false;
            }
            SimpleLogger.Get().Log(Level.INFO, "Execute:skip.. why??");
            return false;

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
                    connector.Reader.Recv();
                }
            }
            this.is_pdu_read_done = true;
        }
        public void WritePdu()
        {
            /********************
             * Onside assets 
             * - Send Sensor Data 
             ********************/
            foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
            {
                //SimpleLogger.Get().Log(Level.INFO, "connector(write):" + connector.GetName());
                if (
                    ((connector.GetName() == null) || connector.GetName().Equals("None"))
                        && (connector.Writer != null))
                {
                    //SimpleLogger.Get().Log(Level.INFO, "WRITE START:" + connector.GetName());
                    connector.Writer.SendWriterPdu();
                    connector.Writer.SendReaderPdu();
                }
            }
            this.is_pdu_write_done = true;
        }

        private void ExecuteSimulation()
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

        public SimulationState GetState()
        {
            return this.attrs.state;
        }

        public long GetWorldTime()
        {
            return this.attrs.master_time;
        }

        public ISimulationAssetManager GetAssetManager()
        {
            return asset_manager;
        }


        public int RefOutsideAssetListCount()
        {
            return 0;
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
        private async Task StartServiceAsync()
        {
            bool ret = RpcClient.Register(my_asset_name, this);
            if (ret != true)
            {
                SimpleLogger.Get().Log(Level.ERROR, "SetInsideWorldSimulator:can not register asset");
            }
            else
            {
                await RpcClient.AssetNotificationStartAsync(my_asset_name);
            }
        }

        public void SetInsideWorldSimulator(IInsideWorldSimulatior isim)
        {
            Task.Run(() => StartServiceAsync());
            this.inside_simulator = isim;
        }

        public bool Start()
        {
            return RpcClient.StartSimulation();
        }

        public bool Stop()
        {
            return RpcClient.StoptSimulation();
        }

        public bool Reset()
        {
            return RpcClient.ResettSimulation();
        }

        public void StartCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "StartCallback");
            inside_asset_list = asset_manager.RefInsideAssetList();
            RpcClient.AssetNotificationFeedbackStart(my_asset_name, true);
        }

        public void StopCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "StopCallback");
            RpcClient.AssetNotificationFeedbackStop(my_asset_name, true);
        }
        private bool has_reset_req = false;
        private void DoResetCallback()
        {
            if (has_reset_req)
            {
                has_reset_req = false;
                PduIoConnector.Reset();
                SimpleLogger.Get().Log(Level.INFO, "ResetCallback:PduIoConnector.Reset(): OK");
                this.RestoreEnvironment();
                SimpleLogger.Get().Log(Level.INFO, "ResetCallback:sim_env.Restore(): OK");
                foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
                {
                    if (connector.Reader != null)
                    {
                        connector.Reader.Reset();
                        SimpleLogger.Get().Log(Level.INFO, "ResetCallback:connector.Reader.Reset(): OK");
                    }
                }
                RpcClient.AssetNotificationFeedbackReset(my_asset_name, true);
            }
        }

        public void ResetCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "ResetCallback");
            has_reset_req = true;
        }
    }
}
#endif