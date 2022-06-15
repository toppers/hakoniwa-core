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
        List<IInsideAssetController> inside_asset_list = null;

        internal static ISimulationController Get(string asset_name)
        {
            simulator.SetAssetName(asset_name);
            return simulator;
        }


        private HakoAssetSimulationController()
        {
            this.asset_manager = new SimulationAssetManager();
            this.sim_env = new SimulationEnvironment();
        }
        private ISimulationAssetManager asset_manager;
        private SimulationEnvironment sim_env;
        private IInsideWorldSimulatior inside_simulator = null;

        private StringBuilder my_asset_name;
        private void SetAssetName(string asset_name)
        {
            this.my_asset_name = new StringBuilder(asset_name);
        }

        public void RegisterEnvironmentOperation(IEnvironmentOperation env_op)
        {
            this.sim_env.Register(env_op);
        }

        public void RestoreEnvironment()
        {
            this.sim_env.Restore();
        }

        public void SaveEnvironment()
        {
            this.sim_env.Save();
        }

        public void SetInsideWorldSimulator(IInsideWorldSimulatior isim)
        {
            HakoCppWrapper.hako_asset_callback_t inputData = new HakoCppWrapper.hako_asset_callback_t()
            {
                start = new HakoCppWrapper.HakoAssetCblkStart(StartCallback),
                stop = new HakoCppWrapper.HakoAssetCblkStop(StopCallback),
                reset = new HakoCppWrapper.HakoAssetCblkReset(ResetCallback)
            };

            IntPtr inputPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(HakoCppWrapper.hako_asset_callback_t)));
            Marshal.StructureToPtr<HakoCppWrapper.hako_asset_callback_t>(inputData, inputPtr, false);
            bool ret = HakoCppWrapper.asset_register(new StringBuilder("TestAsset"), inputPtr);

            this.inside_simulator = isim;
        }
        public long GetWorldTime()
        {
            return HakoCppWrapper.get_wrold_time();
        }
        public bool Execute()
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

            /********************
             * Hakoniwa Time Sync
             ********************/
            bool canStep = HakoCppWrapper.master_execute();
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
            }

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
            return canStep;
        }

        private void StartCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "StartCallback");
            this.inside_asset_list = this.asset_manager.RefInsideAssetList();
            HakoCppWrapper.asset_start_feedback(this.my_asset_name, true);
        }
        private void StopCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "StopCallback");
            HakoCppWrapper.asset_stop_feedback(this.my_asset_name, true);
        }
        private void ResetCallback()
        {
            SimpleLogger.Get().Log(Level.INFO, "ResetCallback");
            PduIoConnector.Reset();
            this.sim_env.Restore();
            foreach (var connector in AssetConfigLoader.RefPduChannelConnector())
            {
                if (connector.Reader != null)
                {
                    connector.Reader.Reset();
                }
            }
            HakoCppWrapper.asset_reset_feedback(this.my_asset_name, true);
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
            return this.asset_manager;
        }


    }
}
