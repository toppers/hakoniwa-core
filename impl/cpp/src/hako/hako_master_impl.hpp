#ifndef _HAKO_MASTER_IMPL_HPP_
#define _HAKO_MASTER_IMPL_HPP_

#include "hako_master.hpp"

namespace hako {
    class HakoMasterControllerImpl : public IHakoMasterController {
    public:
        HakoMasterControllerImpl() {}
        virtual bool execute();
        virtual void set_config_simtime(HakoTimeType max_delay_time, HakoTimeType delta_time);
    private:
        HakoTimeType max_delay_time_;
        HakoTimeType delta_time_;
    };
}

#endif /* _HAKO_MASTER_IMPL_HPP_ */