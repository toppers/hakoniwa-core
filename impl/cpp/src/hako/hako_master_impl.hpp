#ifndef _HAKO_MASTER_IMPL_HPP_
#define _HAKO_MASTER_IMPL_HPP_

#include "hako.hpp"
#include "hako_master.hpp"
#include "data/hako_master_data.hpp"
#include "core/simulation/time/hako_time.hpp"

namespace hako {
    class HakoMasterControllerImpl : public IHakoMasterController {
    public:
        HakoMasterControllerImpl(std::shared_ptr<data::HakoMasterData> ptr)
        {
            this->master_data_ = ptr;
            this->sim_event_ = hako::get_simevent_controller();
        }
        virtual bool execute();
        virtual void set_config_simtime(HakoTimeType max_delay_time_usec, HakoTimeType delta_time_usec);
        virtual HakoTimeType get_max_deltay_time_usec();
        virtual HakoTimeType get_delta_time_usec();
    private:
        HakoMasterControllerImpl() {}
        std::shared_ptr<hako::core::simulation::time::TheWorld> theWorld_;
        std::shared_ptr<data::HakoMasterData> master_data_;
        std::shared_ptr<hako::IHakoSimulationEventController> sim_event_;
    };
}

#endif /* _HAKO_MASTER_IMPL_HPP_ */