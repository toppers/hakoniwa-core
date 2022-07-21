using Hakoniwa.Core.Simulation.Environment;
using System;
using System.Collections.Generic;
using System.Text;

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

    public interface ISimulationController
    {
        void SetInsideWorldSimulator(IInsideWorldSimulatior isim);
        void RegisterEnvironmentOperation(IEnvironmentOperation env_op);
        void SaveEnvironment();
        void RestoreEnvironment();

        long GetWorldTime();
        bool Execute();

        SimulationState GetState();

        bool Start();
        bool Stop();
        bool Reset();

        ISimulationAssetManager GetAssetManager();
        int RefOutsideAssetListCount();
    }
}
