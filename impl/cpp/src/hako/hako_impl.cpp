#include "hako.hpp"
#include "hako_master_impl.hpp"
#include "hako_asset_impl.hpp"

static std::shared_ptr<hako::data::HakoMasterData> master_data_ptr = nullptr;
static std::shared_ptr<hako::IHakoMasterController> master_ptr = nullptr;
static std::shared_ptr<hako::IHakoAssetController> asset_ptr = nullptr;

void hako::init()
{
    if (master_data_ptr == nullptr) {
        master_data_ptr = std::make_shared<hako::data::HakoMasterData>();
        master_data_ptr->init();
    }
    return;
}
void hako::destroy()
{
    if (master_ptr != nullptr) {
        master_ptr = nullptr;
    }
    if (master_data_ptr != nullptr) {
        master_data_ptr->destroy();
        master_data_ptr = nullptr;
    }
    return;
}

std::shared_ptr<hako::IHakoMasterController> hako::create_master()
{
    if (master_data_ptr == nullptr) {
        throw std::invalid_argument("ERROR: hako::init() is not called yet");
    }
    if (master_ptr == nullptr) {
        master_ptr = std::make_shared<hako::HakoMasterControllerImpl>(master_data_ptr);
    }
    return master_ptr;
}

std::shared_ptr<hako::IHakoAssetController> hako::create_asset_controller(int32_t seg_id)
{
    if (master_data_ptr != nullptr) {
        throw std::invalid_argument("ERROR: hako::init() is already called");
    }
    if (asset_ptr != nullptr) {
        return asset_ptr;
    }
    master_data_ptr = std::make_shared<hako::data::HakoMasterData>();
    master_data_ptr->load(seg_id);
    asset_ptr = std::make_shared<hako::HakoAssetControllerImpl>(master_data_ptr);

    return asset_ptr;
}

std::shared_ptr<hako::IHakoSimulationController> hako::get_simulation_controller(int32_t seg_id)
{
    //TODO
    return nullptr;
}
