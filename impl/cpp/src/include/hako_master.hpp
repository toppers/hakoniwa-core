#ifndef _HAKO_MASTER_HPP_
#define _HAKO_MASTER_HPP_

#include "types/hako_types.hpp"

namespace hako {

    class IHakoMasterController {
    public:
        virtual ~IHakoMasterController() {}

        /*
         * Master APIs
         */
        virtual bool execute() = 0;
        virtual void set_config_simtime(HakoTimeType max_delay_time, HakoTimeType delta_time) = 0;
    };
}

#endif /* _HAKO_MASTER_HPP_ */