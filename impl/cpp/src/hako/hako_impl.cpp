#include "hako.hpp"
#include "hako_master_impl.hpp"


void hako::init()
{
    //TODO
    return;
}
std::shared_ptr<hako::IHakoMasterController> hako::create_master()
{
    std::shared_ptr<hako::IHakoMasterController> ptr = std::make_shared<hako::HakoMasterControllerImpl>();
    return ptr;
}

std::shared_ptr<hako::IHakoAssetController> hako::create_asset_controller()
{
    //TODO
    return nullptr;
}

std::shared_ptr<hako::IHakoSimulationController> hako::get_simulation_controller()
{
    //TODO
    return nullptr;
}
