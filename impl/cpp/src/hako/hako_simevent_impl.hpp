#ifndef _HAKO_SIMEVENT_IMPL_HPP_
#define _HAKO_SIMEVENT_IMPL_HPP_

#include "hako_simevent.hpp"
#include "data/hako_master_data.hpp"

namespace hako {
    class HakoSimulationEventController : public IHakoSimulationEventController {
    public:
        HakoSimulationEventController(std::shared_ptr<data::HakoMasterData> ptr)
        {
            this->master_data_ = ptr;
        }

        /*
         * simulation execution event
         */
        bool start();
        bool stop();
        bool reset();

        /*
         * feedback events
         */
        bool start_feedback(const std::string& asset_name, bool isOk);
        bool stop_feedback(const std::string& asset_name, bool isOk);
        bool reset_feedback(const std::string& asset_name, bool isOk);

    private:
        /*
         * monitor
         */
        void run_nolock();
        bool feedback(const std::string& asset_name, bool isOk, HakoSimulationStateType exp_state);
        bool trigger_event(HakoSimulationStateType curr_state, HakoSimulationStateType next_state, hako::data::HakoAssetEventType event);
        HakoSimulationEventController() {}
        std::shared_ptr<data::HakoMasterData> master_data_;
    };    
}

#endif /* _HAKO_SIMEVENT_IMPL_HPP_ */