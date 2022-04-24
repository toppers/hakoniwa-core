#include "hako.hpp"
#include "hako_master_impl.hpp"
#include "hako_asset_impl.hpp"
#include "hako_simevent_impl.hpp"
#include "utils/hako_logger.hpp"
#include "core/context/hako_context.hpp"

static std::shared_ptr<hako::data::HakoMasterData> master_data_ptr = nullptr;
static std::shared_ptr<hako::IHakoMasterController> master_ptr = nullptr;
static std::shared_ptr<hako::IHakoAssetController> asset_ptr = nullptr;
static std::shared_ptr<hako::IHakoSimulationEventController> simevent_ptr = nullptr;

void hako::init()
{
    hako::utils::logger::init("core");
    if (master_data_ptr == nullptr) {
        master_data_ptr = std::make_shared<hako::data::HakoMasterData>();
        master_data_ptr->init();
    }
    hako::utils::logger::get("core")->info("hakoniwa initialized");
    return;
}
void hako::destroy()
{
    hako::core::context::HakoContext context;
    if (master_ptr != nullptr) {
        master_ptr = nullptr;
    }
    if (!context.is_same(master_data_ptr->get_master_pid())) {
        return;
    }
    if (master_data_ptr != nullptr) {
        master_data_ptr->destroy();
        master_data_ptr = nullptr;
    }
    if (asset_ptr != nullptr) {
        asset_ptr = nullptr;
    }
    if (simevent_ptr != nullptr) {
        simevent_ptr = nullptr;
    }
    hako::utils::logger::get("core")->info("hakoniwa destroyed");
    hako::utils::logger::get("core")->flush();
    return;
}

std::shared_ptr<hako::IHakoMasterController> hako::create_master()
{
    HAKO_ASSERT(master_data_ptr != nullptr);
    if (master_ptr == nullptr) {
        master_ptr = std::make_shared<hako::HakoMasterControllerImpl>(master_data_ptr);
    }
    return master_ptr;
}

std::shared_ptr<hako::IHakoAssetController> hako::create_asset_controller()
{
    if (asset_ptr != nullptr) {
        return asset_ptr;
    }
    else if (master_data_ptr == nullptr) {
        master_data_ptr = std::make_shared<hako::data::HakoMasterData>();
        master_data_ptr->load();
    }
    asset_ptr = std::make_shared<hako::HakoAssetControllerImpl>(master_data_ptr);

    return asset_ptr;
}

std::shared_ptr<hako::IHakoSimulationEventController> hako::get_simevent_controller()
{
    if (simevent_ptr != nullptr) {
        return simevent_ptr;
    }
    else if (master_data_ptr == nullptr) {
        master_data_ptr = std::make_shared<hako::data::HakoMasterData>();
        master_data_ptr->load();
    }
    simevent_ptr = std::make_shared<hako::HakoSimulationEventController>(master_data_ptr);

    return simevent_ptr;
}

void hako::logger::init(const std::string &id)
{
    hako::utils::logger::init(id);
}
std::shared_ptr<spdlog::logger> hako::logger::get(const std::string &id)
{
    return hako::utils::logger::get(id);
}
