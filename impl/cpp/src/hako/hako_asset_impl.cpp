#include "hako_asset_impl.hpp"
#include "utils/hako_logger.hpp"
#include "utils/hako_clock.hpp"

bool hako::HakoAssetControllerImpl::asset_register(const std::string & name, AssetCallbackType &callbacks)
{
    auto id = this->master_data_->alloc_asset(name, hako::data::HakoAssetType::HakoAsset_Inside, callbacks);
    if (id < 0) {
        hako::utils::logger::get("core")->error("can not registered: asset[{0}]", name);
        return false;
    }
    else {
        hako::utils::logger::get("core")->info("Registered: asset[{0}]", name);
        return true;
    }
}
bool hako::HakoAssetControllerImpl::asset_remote_register(const std::string & name, AssetCallbackType &callbacks)
{
    auto id = this->master_data_->alloc_asset(name, hako::data::HakoAssetType::HakoAsset_Outside, callbacks);
    if (id < 0) {
        hako::utils::logger::get("core")->error("can not registered: remote asset[{0}]", name);
        return false;
    }
    else {
        hako::utils::logger::get("core")->info("Registered: remote asset[{0}]", name);
    }
    this->remote_event_->start_monitoring(id, callbacks);
    return true;
 }

bool hako::HakoAssetControllerImpl::asset_unregister(const std::string & name)
{
    auto* asset = this->master_data_->get_asset_nolock(name);
    auto ret = this->master_data_->free_asset(name);
    if (ret) {
        if (asset->type == hako::data::HakoAsset_Outside) {
            this->remote_event_->stop_monitoring(asset->id);
        }
        hako::utils::logger::get("core")->info("Unregistered: asset[{0}]", name);
    }
    else {
        hako::utils::logger::get("core")->error("can not unregistered: asset[{0}]", name);
    }
    return ret;
}
void hako::HakoAssetControllerImpl::notify_simtime(const std::string & name, HakoTimeType simtime)
{
    this->master_data_->lock();
    auto* asset = this->master_data_->get_asset_nolock(name);
    if (asset != nullptr) {
        auto* asset_event = this->master_data_->get_asset_event_nolock(asset->id);
        asset_event->ctime = simtime;
        asset_event->update_time = hako_get_clock();
    }
    this->master_data_->unlock();
    return;
}
HakoTimeType hako::HakoAssetControllerImpl::get_worldtime()
{
    hako::data::HakoTimeSetType timeset = this->master_data_->get_time();
    return timeset.current;
}


bool hako::HakoAssetControllerImpl::feedback(const std::string& asset_name, bool isOk, HakoSimulationStateType exp_state)
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        auto* entry = this->master_data_->get_asset_nolock(asset_name);
        if (entry != nullptr) {
            auto* entry_ev = this->master_data_->get_asset_event_nolock(entry->id);
            entry_ev->update_time = hako_get_clock();
            if (state == exp_state) {
                entry_ev->event_feedback = isOk;
            }
            else {
                entry_ev->event_feedback = false;
                ret = false;
            }
        }
        else {
            ret = false;
        }
    }
    this->master_data_->unlock();
    return ret;
}

bool hako::HakoAssetControllerImpl::start_feedback(const std::string& asset_name, bool isOk)
{
    return this->feedback(asset_name, isOk, HakoSim_Runnable);
}
bool hako::HakoAssetControllerImpl::stop_feedback(const std::string& asset_name, bool isOk)
{
    return this->feedback(asset_name, isOk, HakoSim_Stopping);
}
bool hako::HakoAssetControllerImpl::reset_feedback(const std::string& asset_name, bool isOk)
{
    return this->feedback(asset_name, isOk, HakoSim_Resetting);
}
