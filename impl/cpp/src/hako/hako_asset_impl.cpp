#include "hako_asset_impl.hpp"
#include "utils/hako_logger.hpp"

bool hako::HakoAssetControllerImpl::asset_register(const std::string & name, AssetCallbackType &callbacks)
{
    auto id = this->master_data_->alloc_asset(name, hako::data::HakoAssetType::HakoAsset_Inside, callbacks);
    if (id < 0) {
        hako::utils::logger::get()->error("can not registered: asset[{0}]", name);
        return false;
    }
    else {
        hako::utils::logger::get()->info("Registered: asset[{0}]", name);
        return true;
    }
}
bool hako::HakoAssetControllerImpl::asset_unregister(const std::string & name)
{
    auto ret = this->master_data_->free_asset(name);
    if (ret) {
        hako::utils::logger::get()->info("Unregistered: asset[{0}]", name);
    }
    else {
        hako::utils::logger::get()->error("can not unregistered: asset[{0}]", name);
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
    }
    this->master_data_->unlock();
    return;
}
HakoTimeType hako::HakoAssetControllerImpl::get_worldtime()
{
    hako::data::HakoTimeSetType timeset = this->master_data_->get_time();
    return timeset.current;
}
