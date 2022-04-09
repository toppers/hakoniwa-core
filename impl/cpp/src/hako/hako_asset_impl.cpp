#include "hako_asset_impl.hpp"

bool hako::HakoAssetControllerImpl::asset_register(const std::string & name, AssetCallbackType &callbacks)
{
    auto id = this->master_data_->alloc_asset(name, hako::data::HakoAssetType::HakoAsset_Inside, callbacks);
    if (id < 0) {
        return false;
    }
    return true;
}
bool hako::HakoAssetControllerImpl::asset_unregister(const std::string & name)
{
    return this->master_data_->free_asset(name);
}
void hako::HakoAssetControllerImpl::notify_simtime(HakoTimeType simtime)
{
    //TODO
    this->master_data_->lock();
    auto& timeset = this->master_data_->ref_time_nolock();
    timeset.current = simtime;
    this->master_data_->unlock();
    return;
}
HakoTimeType hako::HakoAssetControllerImpl::get_worldtime()
{
    hako::data::HakoTimeSetType timeset = this->master_data_->get_time();
    return timeset.current;
}
