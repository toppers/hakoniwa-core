#ifndef _HAKO_TIME_HPP_
#define _HAKO_TIME_HPP_

#include "types/hako_types.hpp"
#include <vector>

namespace hako::core::simulation::time {

    class TheWorld {
    public:
        TheWorld(HakoTimeType max, HakoTimeType delta) {
            this->max_delay_time_usec = max;
            this->delta_time_usec = delta;
            this->wait_time_usec = 0;
            this->world_time_usec = 0;
        }
        ~TheWorld() {}

        HakoTimeType time_begins_to_move(std::vector<HakoTimeType> &asset_times);
        HakoTimeType get_world_time_usec()
        {
            return this->world_time_usec;
        }
        HakoTimeType get_wait_time_usec()
        {
            return this->wait_time_usec;
        }
    private:
        TheWorld() {}
        HakoTimeType max_delay_time_usec;
        HakoTimeType delta_time_usec;
        HakoTimeType wait_time_usec;
        HakoTimeType world_time_usec;
    };

}

#endif /* _HAKO_TIME_HPP_ */