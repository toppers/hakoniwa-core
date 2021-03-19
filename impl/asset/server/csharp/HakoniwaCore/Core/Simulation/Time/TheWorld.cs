using Hakoniwa.Core.Asset;
using Hakoniwa.PluggableAsset.Assets;
using System;
using System.Collections.Generic;

namespace Hakoniwa.Core.Simulation.Time
{
    class TheWorld
    {
        private long world_time = 0; /* usec */
        private long max_delay_time = 0; /* usec */
        public long wait_time = 0; /* usec */
        private long delta_time = 0; /* usec */
        public TheWorld()
        {
        }
        public void SetMaxDelayTime(long max)
        {
            this.max_delay_time = max;
        }
        public void SetDeltaTime(long dtime)
        {
            this.delta_time = dtime;
        }
        public bool CanStep(List<IOutsideAssetController> assets)
        {
            bool isConnected = true;
            bool canStep = true;
            foreach (var outside_asset in assets)
            {
                max_delay_time = outside_asset.GetSimTime();
                if (max_delay_time <= -this.max_delay_time)
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
                this.wait_time++;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void StepFoward()
        {
            this.world_time++;
        }

        internal long GetWorldTime()
        {
            return this.world_time;
        }
    }
}
