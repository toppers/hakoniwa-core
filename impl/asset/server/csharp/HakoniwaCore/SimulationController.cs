using HakoniwaGrpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core
{
    public enum HakoniwaSimulationState
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
        private Object lockObj = new Object();
        public HakoniwaSimulationState state = HakoniwaSimulationState.Stopped;
        public HakoniwaSimulationResult result = HakoniwaSimulationResult.Success;

        private int asset_feedback_count;
        public AssetManager asset_mgr = new AssetManager();
        public RequestManager req_mgr = new RequestManager();

        public SimulationController()
        {
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
        private void PublishEvent(AssetNotificationEvent ev)
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
                if (state == HakoniwaSimulationState.Stopped)
                {
                    state = HakoniwaSimulationState.Runnable;
                    PublishEvent(AssetNotificationEvent.Start);
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
                if (state == HakoniwaSimulationState.Runnable)
                {
                    if (AssetFeedback(isStarted))
                    {
                        Console.WriteLine("StateChanged:" + state);
                        state = HakoniwaSimulationState.Running;
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
                if (state == HakoniwaSimulationState.Running)
                {
                    state = HakoniwaSimulationState.Stopping;
                    PublishEvent(AssetNotificationEvent.End);
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
                if (state == HakoniwaSimulationState.Stopping)
                {
                    if (AssetFeedback(isStopped))
                    {
                        state = HakoniwaSimulationState.Stopped;
                        Console.WriteLine("StateChanged:" + state);
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
                if (state == HakoniwaSimulationState.Stopped)
                {
                    state = HakoniwaSimulationState.Terminated;
                    PublishEvent(AssetNotificationEvent.None);
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

        public HakoniwaSimulationState GetState()
        {
            return state;
        }
    }
}
