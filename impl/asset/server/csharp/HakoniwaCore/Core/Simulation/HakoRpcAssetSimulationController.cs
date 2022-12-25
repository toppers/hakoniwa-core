using Hakoniwa.Core.Rpc;
using Hakoniwa.Core.Simulation.Environment;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset;
using Hakoniwa.PluggableAsset.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation
{
    class HakoRpcAssetSimulationController : ISimulationController
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
        }

        private ISimulationAssetManager asset_manager;
        private SimulationEnvironment sim_env;
        private IInsideWorldSimulatior inside_simulator = null;
        private long asset_time_usec = 0;

        //TODO created_pdu && sync_mode check
        public bool Execute()
        {
            //for heartbeat
            RpcClient.NotifySimtime(my_asset_name, asset_time_usec);

            if (this.GetState() != SimulationState.Running)
            {
                return false;
            }

            /********************
             * Hakoniwa Time Sync
             ********************/
            long world_time = this.GetWorldTime();
            if (this.asset_time_usec < world_time)
            {
                this.asset_time_usec += this.inside_simulator.GetDeltaTimeUsec();
                RpcClient.NotifySimtime(my_asset_name, asset_time_usec);
            }
            else
            {
                // can not do simulation because world time is slow...
                //SimpleLogger.Get().Log(Level.INFO, "Execute:skip.. asset_time={0} world_time={1}", this.asset_time_usec, world_time);
                return false;
            }
            this.ReadPdu();
            this.ExecuteSimulation();
            this.WritePdu();
            return true;
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

            //TODO HakoCppWrapper.asset_notify_read_pdu_done(my_asset_name);
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
                    connector.Writer.SendWriterPdu();
                    connector.Writer.SendReaderPdu();
                }
            }
            //TODO HakoCppWrapper.asset_notify_write_pdu_done(my_asset_name);
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

        //TODO
        public SimulationState GetState()
        {
            throw new NotImplementedException();
        }

        //TODO
        public long GetWorldTime()
        {
            throw new NotImplementedException();
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

        public void SetInsideWorldSimulator(IInsideWorldSimulatior isim)
        {
            bool ret = RpcClient.Register(my_asset_name);
            if (ret != true)
            {
                SimpleLogger.Get().Log(Level.ERROR, "SetInsideWorldSimulator:can not register asset");
            }
            this.inside_simulator = isim;
        }

        //TODO
        public bool Start()
        {
            throw new NotImplementedException();
        }

        //TODO
        public bool Stop()
        {
            throw new NotImplementedException();
        }
        //TODO
        public bool Reset()
        {
            throw new NotImplementedException();
        }
    }
}
