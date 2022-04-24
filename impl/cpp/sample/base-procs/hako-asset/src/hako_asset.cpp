#include <hako.hpp>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <signal.h>

static std::string *asset_name = nullptr;
static bool hako_asset_is_end = false;
static HakoTimeType hako_asset_time_usec = 0LL;

static std::shared_ptr<hako::IHakoSimulationEventController> hako_sim_ctrl = nullptr;
static std::shared_ptr<hako::IHakoAssetController> hako_asset = nullptr;

static void reset_callback()
{
    hako_asset->reset_feedback(*asset_name, true);
}
static void start_callback()
{
    hako_asset->start_feedback(*asset_name, true);
}
static void stop_callback()
{
    hako_asset->stop_feedback(*asset_name, true);
}

static void hako_asset_signal_handler(int sig)
{
    if (asset_name != nullptr) {
        hako::logger::get(*asset_name)->info("SIGNAL RECV: {0}", sig);
    }
    hako_asset_is_end = true;
}

int main(int argc, const char* argv[])
{
    if (argc != 3) {
        printf("Usage: %s <delta_msec> <asset_name>\n", argv[0]);
        return 1;
    }
    printf("START\n");
    signal(SIGINT, hako_asset_signal_handler);
    signal(SIGTERM, hako_asset_signal_handler);

    HakoTimeType delta_usec = strtol(argv[1], NULL, 10) * 1000;
    std::string asset_name_str = argv[2];
    asset_name = &asset_name_str;

    hako::logger::init("core");
    hako::logger::init(asset_name_str);
    hako::logger::get(asset_name_str)->info("delta={0} usec asset_name={1}", delta_usec, asset_name_str);

    hako_asset = hako::create_asset_controller();

    AssetCallbackType callback;
    callback.reset = reset_callback;
    callback.start = start_callback;
    callback.stop = stop_callback;
    bool ret = hako_asset->asset_register(asset_name_str, callback);
    if (ret == false) {
        hako::logger::get(asset_name_str)->error("Can not register");
        return 1;
    }
    
    while (hako_asset_is_end == false) {
        hako_asset->notify_simtime(asset_name_str, hako_asset_time_usec);
        HakoTimeType world_time = hako_asset->get_worldtime();
        if (world_time >= hako_asset_time_usec) {
            hako_asset_time_usec += delta_usec;
        }
        printf("TIME: W:%ld A:%ld\n", world_time, hako_asset_time_usec);
        usleep(delta_usec);
    }
    hako_asset->asset_unregister(asset_name_str);
    hako::logger::get(asset_name_str)->flush();

    printf("EXIT\n");
    return 0;
}
