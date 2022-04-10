#include "hako_simevent_impl.hpp"

bool hako::HakoSimulationEventController::start()
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        if (state == HakoSim_Stopped) {
            state = HakoSim_Runnable;
        }
        else {
            ret = false;
        }
    }
    this->master_data_->unlock();

    this->master_data_->publish_event_nolock(hako::data::HakoAssetEvent_Start);
    return ret;
}

bool hako::HakoSimulationEventController::start_feedback(const std::string& asset_name, bool isOk)
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        if (state == HakoSim_Runnable) {
            //TODO
            auto* entry = this->master_data_->get_asset(asset_name);
            if (entry != nullptr) {
                auto* entry_ev = this->master_data_->get_asset_event(entry->id);
                entry_ev->event_feedback = isOk;
            }
            else {
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

bool hako::HakoSimulationEventController::stop()
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        if (state == HakoSim_Running) {
            state = HakoSim_Stopping;
        }
        else {
            ret = false;
        }
    }
    this->master_data_->unlock();

    this->master_data_->publish_event_nolock(hako::data::HakoAssetEvent_Stop);

    return ret;
}

bool hako::HakoSimulationEventController::stop_feedback(const std::string& asset_name, bool isOk)
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        if (state == HakoSim_Stopping) {
            //TODO
            auto* entry = this->master_data_->get_asset(asset_name);
            if (entry != nullptr) {
                auto* entry_ev = this->master_data_->get_asset_event(entry->id);
                entry_ev->event_feedback = isOk;
            }
            else {
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
bool hako::HakoSimulationEventController::reset()
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        if (state == HakoSim_Stopped) {
            state = HakoSim_Resetting;
        }
        else {
            ret = false;
        }
    }
    this->master_data_->unlock();

    this->master_data_->publish_event_nolock(hako::data::HakoAssetEvent_Reset);
    return ret;
}
bool hako::HakoSimulationEventController::reset_feedback(const std::string& asset_name, bool isOk)
{
    bool ret = true;
    this->master_data_->lock();
    {
        auto& state = this->master_data_->ref_state_nolock();
        if (state == HakoSim_Resetting) {
            //TODO
            auto* entry = this->master_data_->get_asset(asset_name);
            if (entry != nullptr) {
                auto* entry_ev = this->master_data_->get_asset_event(entry->id);
                entry_ev->event_feedback = isOk;
            }
            else {
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