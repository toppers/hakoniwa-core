#include "hako_simevent_impl.hpp"

bool hako::HakoSimulationEventController::trigger_event(HakoSimulationStateType curr_state, HakoSimulationStateType next_state, hako::data::HakoAssetEventType event)
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        if (state == curr_state) {
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
        auto* entry = this->master_data_->get_asset(asset_name);
        if (entry != nullptr) {
            auto* entry_ev = this->master_data_->get_asset_event(entry->id);
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
        this->run_nolock();
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
    return this->trigger_event(HakoSim_Stopped, HakoSim_Resetting, hako::data::HakoAssetEvent_Reset);
}
bool hako::HakoSimulationEventController::reset_feedback(const std::string& asset_name, bool isOk)
{
    return this->feedback(asset_name, isOk, HakoSim_Resetting);
}

void hako::HakoSimulationEventController::run_nolock()
{
    auto& state = this->master_data_->ref_state_nolock();
    switch (state) {
        case HakoSim_Runnable:
            if (this->master_data_->is_all_feedback_nolock()) {
                state = HakoSim_Running;
            }
            break;
        case HakoSim_Stopping:
        case HakoSim_Resetting:
            if (this->master_data_->is_all_feedback_nolock()) {
                state = HakoSim_Stopped;
            }
            break;
        case HakoSim_Running:
            /* todo */
            break;
        case HakoSim_Stopped:
        case HakoSim_Terminated:
        default:
            break;
    }

    return;
}