#include "core/asset/hako_remote_asset_event.hpp"
#include "utils/hako_sem.hpp"
#include "utils/hako_logger.hpp"

void hako::core::asset::HakoRemoteAssetEvent::monitor_thread(HakoAssetIdType asset_id, AssetCallbackType *callbacks)
{
    hako::utils::logger::get("core")->info("HakoRemoteAssetEvent: monitor_thread start: asset[{0}]", asset_id);
    while (true) {
        hako::utils::sem::asset_down(this->master_data_->get_semid(), asset_id);
        auto* asset = this->master_data_->get_asset_event_nolock(asset_id);
        if (asset == nullptr) {
            break;
        }

        if (asset->event == hako::data::HakoAssetEvent_Start) {
            callbacks->start();
        }
        else if (asset->event == hako::data::HakoAssetEvent_Stop) {
            callbacks->stop();
        }
        else if (asset->event == hako::data::HakoAssetEvent_Reset) {
            callbacks->reset();
        }
    }
    hako::utils::logger::get("core")->info("HakoRemoteAssetEvent: monitor_thread stop: asset[{0}]", asset_id);
    return;
}

void hako::core::asset::HakoRemoteAssetEvent::start_monitoring(HakoAssetIdType asset_id, AssetCallbackType &callbacks)
{
    std::shared_ptr<std::thread> thrp = std::make_shared<std::thread>(&hako::core::asset::HakoRemoteAssetEvent::monitor_thread, this, asset_id, &callbacks);
    thrp->detach();
    this->map_.insert(std::make_pair(asset_id, thrp));
}

void hako::core::asset::HakoRemoteAssetEvent::stop_monitoring(HakoAssetIdType asset_id)
{
    hako::utils::sem::asset_up(this->master_data_->get_semid(), asset_id);
    std::shared_ptr<std::thread> thrp = this->map_[asset_id];
    thrp->join();
    return;
}