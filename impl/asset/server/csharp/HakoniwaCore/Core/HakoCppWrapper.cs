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
        static extern bool hako_asset_register(string name, IntPtr callbacks);
        static public bool asset_register(string name, IntPtr callbacks)
        {
            return HakoCppWrapper.hako_asset_register(name, callbacks);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_register_polling(string name);
        static public bool asset_register_polling(string name)
        {
            return HakoCppWrapper.hako_asset_register_polling(name);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern int hako_asset_get_event(string name);
        public enum HakoSimAssetEvent
        {
            HakoSimAssetEvent_None = 0,
            HakoSimAssetEvent_Start = 1,
            HakoSimAssetEvent_Stop = 2,
            HakoSimAssetEvent_Reset = 3,
            HakoSimAssetEvent_Error = 4,
            HakoSimAssetEvent_Count = 5
        }
        static public HakoSimAssetEvent asset_get_event(string name)
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
        static extern bool hako_asset_unregister(string name);
        static public bool asset_unregister(string asset_name)
        {
            return HakoCppWrapper.hako_asset_unregister(asset_name);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern void hako_asset_notify_simtime(string name, long simtime);
        static public void asset_notify_simtime(string asset_name, long simtime)
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
        static extern bool hako_asset_start_feedback(string asset_name, bool isOk);
        static public bool asset_start_feedback(string asset_name, bool isOk)
        {
            return HakoCppWrapper.hako_asset_start_feedback(asset_name, isOk);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_stop_feedback(string asset_name, bool isOk);
        static public bool asset_stop_feedback(string asset_name, bool isOk)
        {
            return HakoCppWrapper.hako_asset_stop_feedback(asset_name, isOk);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_reset_feedback(string asset_name, bool isOk);
        static public bool asset_reset_feedback(string asset_name, bool isOk)
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
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_create_pdu_lchannel(string robo_name, int channel_id, uint pdu_size);
        static public bool asset_create_pdu_lchannel(string robo_name, int channel_id, uint pdu_size)
        {
            return HakoCppWrapper.hako_asset_create_pdu_lchannel(robo_name, channel_id, pdu_size);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_is_pdu_dirty(string asset_name, string robo_name, int channel_id);
        static public bool asset_is_pdu_dirty(string asset_name, string robo_name, int channel_id)
        {
            return HakoCppWrapper.hako_asset_is_pdu_dirty(asset_name, robo_name, channel_id);
        }

        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_write_pdu(string asset_name, string robo_name, int channel_id, IntPtr pdu_data, uint len);
        static public bool asset_write_pdu(string asset_name, string robo_name, int channel_id, IntPtr pdu_data, uint len)
        {
            return HakoCppWrapper.hako_asset_write_pdu(asset_name, robo_name, channel_id, pdu_data, len);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_read_pdu(string asset_name, string robo_name, int channel_id, IntPtr pdu_data, uint len);
        static public bool asset_read_pdu(string asset_name, string robo_name, int channel_id, IntPtr pdu_data, uint len)
        {
            return HakoCppWrapper.hako_asset_read_pdu(asset_name, robo_name, channel_id, pdu_data, len);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern void hako_asset_notify_read_pdu_done(string asset_name);
        static public void asset_notify_read_pdu_done(string asset_name)
        {
            HakoCppWrapper.hako_asset_notify_read_pdu_done(asset_name);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern void hako_asset_notify_write_pdu_done(string asset_name);
        static public void asset_notify_write_pdu_done(string asset_name)
        {
            HakoCppWrapper.hako_asset_notify_write_pdu_done(asset_name);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_is_pdu_sync_mode(string asset_name);
        static public bool asset_is_pdu_sync_mode(string asset_name)
        {
            return HakoCppWrapper.hako_asset_is_pdu_sync_mode(asset_name);
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_is_simulation_mode();
        static public bool asset_is_simulation_mode()
        {
            return HakoCppWrapper.hako_asset_is_simulation_mode();
        }
        [DllImport("libshakoc", CallingConvention = CallingConvention.Cdecl)]
        static extern bool hako_asset_is_pdu_created();
        static public bool asset_is_pdu_created()
        {
            return HakoCppWrapper.hako_asset_is_pdu_created();
        }

    }
}
