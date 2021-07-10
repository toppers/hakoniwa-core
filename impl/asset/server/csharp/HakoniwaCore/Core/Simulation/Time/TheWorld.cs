using Hakoniwa.Core.Asset;
using Hakoniwa.Core.Utils.Logger;
using Hakoniwa.PluggableAsset.Assets;
using System;
using System.Collections.Generic;

namespace Hakoniwa.Core.Simulation.Time
{
    class TheWorld
    {
        private long world_time = 1; /* usec */
        private long max_delay_time = 0; /* usec */
        public long wait_time = 0; /* usec */
        private long delta_time = 0; /* usec */
        public TheWorld()
        {
        }

        private long saved_world_time;
        private long saved_wait_time;
        public void Save()
        {
            this.saved_wait_time = this.wait_time;
            this.saved_world_time = this.world_time;
        }
        public void Restore()
        {
            this.wait_time = this.saved_wait_time;
            this.world_time = this.saved_world_time;
        }
        public void SetMaxDelayTime(long max)
        {
            this.max_delay_time = max;
        }
        public void SetDeltaTime(long dtime)
        {
            this.delta_time = dtime;
        }
        public long GetDeltaTime()
        {
            return this.delta_time;
        }
        public long GetWaitTime()
        {
            return this.wait_time;
        }
        public bool CanStep(List<IOutsideAssetController> assets)
        {
            bool isConnected = true;
            bool canStep = true;
            foreach (var outside_asset in assets)
            {
                long diff = outside_asset.GetSimTime() - this.world_time;
                //SimpleLogger.Get().Log(Level.DEBUG, "micon=" + outside_asset.GetSimTime());
                //SimpleLogger.Get().Log(Level.DEBUG, "world_time=" + world_time);
                if (diff <= -this.max_delay_time)
                {
                    canStep = false;
                }
                if (!outside_asset.IsConnected())
                {
                    isConnected = false;
                }
            }
            if (isConnected && canStep)
            {
                return true;
            }
            else
            {
                this.wait_time += this.delta_time;
                //SimpleLogger.Get().Log(Level.DEBUG, "wait_time=" + wait_time);
                return false;
            }
        }
        public void StepFoward()
        {
            this.world_time += this.delta_time;
            //Debug.Log("world_time=" + world_time +"delta_time=" + this.delta_time);
        }

        internal long GetWorldTime()
        {
            return this.world_time;
        }
    }
}
