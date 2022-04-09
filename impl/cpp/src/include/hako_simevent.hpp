#ifndef _HAKO_SIMEVENT_HPP_
#define _HAKO_SIMEVENT_HPP_

#include "types/hako_types.hpp"

namespace hako {

    class IHakoSimulationEventController {
    public:
        virtual ~IHakoSimulationEventController() {}

        /*
         * simulation execution event
         */
        virtual bool start() = 0;
        virtual bool stop() = 0;
        virtual bool reset() = 0;

        /*
         * feedback events
         */
        virtual bool start_feedback(const std::string& asset_name, bool isOk) = 0;
        virtual bool stop_feedback(const std::string& asset_name, bool isOk) = 0;
        virtual bool reset_feedback(const std::string& asset_name, bool isOk) = 0;
    };
}

#endif /* _HAKO_SIMEVENT_HPP_ */