#include "hako_simevent_impl.hpp"
#include "utils/hako_logger.hpp"

HakoSimulationStateType hako::HakoSimulationEventController::state()
{
    auto& state = this->master_data_->ref_state_nolock();
    return state;
}

bool hako::HakoSimulationEventController::trigger_event(HakoSimulationStateType curr_state, HakoSimulationStateType next_state, hako::data::HakoAssetEventType event)
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        if (state == curr_state) {
            state = next_state;
        }
        else if (next_state == HakoSim_Error) {
            state = next_state;
        }
        else {
            ret = false;
        }
    }
    this->master_data_->unlock();

    if (ret) {
        this->master_data_->publish_event_nolock(event);
    }
    return ret;
}

bool hako::HakoSimulationEventController::start()
{
    return this->trigger_event(HakoSim_Stopped, HakoSim_Runnable, hako::data::HakoAssetEvent_Start);
}


bool hako::HakoSimulationEventController::feedback(const std::string& asset_name, bool isOk, HakoSimulationStateType exp_state)
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
        this->do_event_handling_nolock(nullptr);
    }
    this->master_data_->unlock();
    return ret;
}


bool hako::HakoSimulationEventController::start_feedback(const std::string& asset_name, bool isOk)
{
    return this->feedback(asset_name, isOk, HakoSim_Runnable);
}

bool hako::HakoSimulationEventController::stop()
{
    return this->trigger_event(HakoSim_Running, HakoSim_Stopping, hako::data::HakoAssetEvent_Stop);
}

bool hako::HakoSimulationEventController::stop_feedback(const std::string& asset_name, bool isOk)
{
    return this->feedback(asset_name, isOk, HakoSim_Stopping);
}
bool hako::HakoSimulationEventController::reset()
{
    auto& state = this->master_data_->ref_state_nolock();
    if (state == HakoSim_Stopped) {
        return this->trigger_event(HakoSim_Stopped, HakoSim_Resetting, hako::data::HakoAssetEvent_Reset);
    }
    else if (state == HakoSim_Error) {
        return this->trigger_event(HakoSim_Error, HakoSim_Stopped, hako::data::HakoAssetEvent_Reset);
    }
    return false;
}
bool hako::HakoSimulationEventController::reset_feedback(const std::string& asset_name, bool isOk)
{
    return this->feedback(asset_name, isOk, HakoSim_Resetting);
}
void hako::HakoSimulationEventController::do_event_handling()
{
    std::vector<HakoAssetIdType> error_assets;
    this->master_data_->lock();
    this->do_event_handling_nolock(&error_assets);
    this->master_data_->unlock();

    if (error_assets.size() > 0) {
        (void)this->trigger_event(HakoSim_Any, HakoSim_Error, hako::data::HakoAssetEvent_Error);
    }

    for (auto& asset_id : error_assets) {
        hako::data::HakoAssetEntryType *entry = this->master_data_->get_asset(asset_id);
        std::shared_ptr<std::string>  asset_name = hako::utils::hako_fixed2string(entry->name);
        hako::utils::logger::get()->error("asset[{0}] timeout", *asset_name);
        this->asset_controller_->asset_unregister(*asset_name);
    }
}
void hako::HakoSimulationEventController::do_event_handling_nolock(std::vector<HakoAssetIdType> *error_assets)
{
    auto& state = this->master_data_->ref_state_nolock();
    switch (state) {
        case HakoSim_Runnable:
            if (this->master_data_->is_all_feedback_nolock()) {
                state = HakoSim_Running;
            }
            else if (error_assets != nullptr) {
                this->do_event_handling_timeout_nolock(error_assets);
            }
            break;
        case HakoSim_Stopping:
        case HakoSim_Resetting:
            if (this->master_data_->is_all_feedback_nolock()) {
                state = HakoSim_Stopped;
            }
            else if (error_assets != nullptr) {
                this->do_event_handling_timeout_nolock(error_assets);
            }
            break;
        case HakoSim_Running:
            /* nothing to do */
            break;
        case HakoSim_Stopped:
        case HakoSim_Terminated:
        default:
            break;
    }

    return;
}
void hako::HakoSimulationEventController::do_event_handling_timeout_nolock(std::vector<HakoAssetIdType> *error_assets)
{
    if (error_assets == nullptr) {
        return;
    }
    for (int id = 0; id < HAKO_DATA_MAX_ASSET_NUM; id++)
    {
        auto* asset_ev = this->master_data_->get_asset_event_nolock(id);
        if (asset_ev != nullptr) {
            if (this->master_data_->is_asset_timeout_nolock(id)) {
                //hako::utils::logger::get()->info("TIMEOUT:{0}", id);
                error_assets->push_back(id);
            }
        }
    }
}