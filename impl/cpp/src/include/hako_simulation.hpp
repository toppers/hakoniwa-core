#ifndef _HAKO_SIMULATION_HPP_
#define _HAKO_SIMULATION_HPP_

#include "types/hako_types.hpp"

namespace hako {

    class IHakoSimulationController {
    public:
        virtual ~IHakoSimulationController() {}

        /*
         * Simulation APIs
         */
        virtual HakoSimulationStateType get_state() = 0;
        virtual HakoTimeType get_worldtime() = 0;
        virtual bool start() = 0;
        virtual bool stop() = 0;
        virtual bool reset() = 0;
    };
}

#endif /* _HAKO_SIMULATION_HPP_ */