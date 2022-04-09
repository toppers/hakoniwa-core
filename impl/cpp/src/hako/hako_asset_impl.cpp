#include "hako_asset_impl.hpp"

bool hako::HakoAssetControllerImpl::asset_register(const std::string & name, AssetCallbackType &callbacks)
{
    //TODO
    return true;
}
bool hako::HakoAssetControllerImpl::asset_unregister(const std::string & name)
{
    //TODO
    return true;
}
void hako::HakoAssetControllerImpl::notify_simtime(HakoTimeType simtime)
{
    //TODO
    return;
}
HakoTimeType hako::HakoAssetControllerImpl::get_worldtime()
{
    hako::data::HakoTimeSetType timeset = this->master_data_->get_time();
    return timeset.current;
}
