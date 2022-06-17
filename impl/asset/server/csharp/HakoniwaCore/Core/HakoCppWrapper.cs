using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Hakoniwa.Core
{
    public class HakoCppWrapper
    {
        public delegate void HakoAssetCblkStart();
        public delegate void HakoAssetCblkStop();
        public delegate void HakoAssetCblkReset();

        private HakoCppWrapper()
        {
        }

        /*
         * for master
         */ 
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_master_init();
        static public bool master_init()
        {
            return HakoCppWrapper.hako_master_init();
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_master_execute();
        static public bool master_execute()
        {
            return HakoCppWrapper.hako_master_execute();
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern void hako_master_set_config_simtime(long max_delay_time_usec, long delta_time_usec);
        static public void master_set_config_simtime(long max_delay_time_usec, long delta_time_usec)
        {
            HakoCppWrapper.hako_master_set_config_simtime(max_delay_time_usec, delta_time_usec);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern long hako_master_get_max_deltay_time_usec();
        static public long master_get_max_delta_time_usec()
        {
            return HakoCppWrapper.hako_master_get_max_deltay_time_usec();
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern long hako_master_get_delta_time_usec();
        static public long master_get_delta_time_usec()
        {
            return HakoCppWrapper.hako_master_get_delta_time_usec();
        }

        /*
         *  for asset
         */
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_init();
        static public bool asset_init()
        {
            return HakoCppWrapper.hako_asset_init();
        }
        public struct hako_asset_callback_t
        {
            public HakoAssetCblkStart start;
            public HakoAssetCblkStop stop;
            public HakoAssetCblkReset reset;

        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_register(StringBuilder name, IntPtr callbacks);
        static public bool asset_register(StringBuilder name, IntPtr callbacks)
        {
            return HakoCppWrapper.hako_asset_register(name, callbacks);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_register_polling(StringBuilder name);
        static public bool asset_register_polling(StringBuilder name)
        {
            return HakoCppWrapper.hako_asset_register_polling(name);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern int hako_asset_get_event(StringBuilder name);
        public enum HakoSimAssetEvent
        {
            HakoSimAssetEvent_None = 0,
            HakoSimAssetEvent_Start = 1,
            HakoSimAssetEvent_Stop = 2,
            HakoSimAssetEvent_Reset = 3,
            HakoSimAssetEvent_Error = 4,
            HakoSimAssetEvent_Count = 5
        }
        static public HakoSimAssetEvent asset_get_event(StringBuilder name)
        {
            int ev = HakoCppWrapper.hako_asset_get_event(name);
            //Hakoniwa.Core.Utils.Logger.SimpleLogger.Get().Log(Hakoniwa.Core.Utils.Logger.Level.INFO, "asset_get_event():" + ev);
            switch (ev)
            {
                case 0:
                    return HakoSimAssetEvent.HakoSimAssetEvent_None;
                case 1:
                    return HakoSimAssetEvent.HakoSimAssetEvent_Start;
                case 2:
                    return HakoSimAssetEvent.HakoSimAssetEvent_Stop;
                case 3:
                    return HakoSimAssetEvent.HakoSimAssetEvent_Reset;
                default:
                    return HakoSimAssetEvent.HakoSimAssetEvent_Error;
            }
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_unregister(StringBuilder name);
        static public bool asset_unregister(StringBuilder asset_name)
        {
            return HakoCppWrapper.hako_asset_unregister(asset_name);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern void hako_asset_notify_simtime(StringBuilder name, long simtime);
        static public void asset_notify_simtime(StringBuilder asset_name, long simtime)
        {
            HakoCppWrapper.hako_asset_notify_simtime(asset_name, simtime);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern long hako_asset_get_worldtime();
        static public long get_wrold_time()
        {
            return HakoCppWrapper.hako_asset_get_worldtime();
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_start_feedback(StringBuilder asset_name, bool isOk);
        static public bool asset_start_feedback(StringBuilder asset_name, bool isOk)
        {
            return HakoCppWrapper.hako_asset_start_feedback(asset_name, isOk);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_stop_feedback(StringBuilder asset_name, bool isOk);
        static public bool asset_stop_feedback(StringBuilder asset_name, bool isOk)
        {
            return HakoCppWrapper.hako_asset_stop_feedback(asset_name, isOk);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_reset_feedback(StringBuilder asset_name, bool isOk);
        static public bool asset_reset_feedback(StringBuilder asset_name, bool isOk)
        {
            return HakoCppWrapper.hako_asset_reset_feedback(asset_name, isOk);
        }

        /*
         * for simevent
         */
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_simevent_init();
        static public bool simevent_init()
        {
            return HakoCppWrapper.hako_simevent_init();
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern int hako_simevent_get_state();
        public enum HakoState
        {
            Stopped = 0,
            Runnable = 1,
            Running = 2,
            Stopping = 3,
            Resetting = 4,
            Error = 5,
            Terminated = 6,
        }
        static public HakoState simevent_get_state()
        {
            switch (HakoCppWrapper.hako_simevent_get_state())
            {
                case 0:
                    return HakoState.Stopped;
                case 1:
                    return HakoState.Runnable;
                case 2:
                    return HakoState.Running;
                case 3:
                    return HakoState.Stopping;
                case 4:
                    return HakoState.Resetting;
                case 5:
                    return HakoState.Error;
                case 6:
                    return HakoState.Terminated;
                default:
                    return HakoState.Terminated;
            }
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_simevent_start();
        static public bool simevent_start()
        {
            return HakoCppWrapper.hako_simevent_start();
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_simevent_stop();
        static public bool simevent_stop()
        {
            return HakoCppWrapper.hako_simevent_stop();
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_simevent_reset();
        static public bool simevent_reset()
        {
            return HakoCppWrapper.hako_simevent_reset();
        }
    }
}
