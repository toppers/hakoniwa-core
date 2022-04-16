#ifndef _HAKO_SIMEVENT_HPP_
#define _HAKO_SIMEVENT_HPP_

#include "types/hako_types.hpp"

namespace hako {

    class IHakoSimulationEventController {
    public:
        virtual ~IHakoSimulationEventController() {}

        virtual HakoSimulationStateType state() = 0;
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

        /*
         * event monitor
         */
        virtual void do_event_handling() = 0;
    };
}

#endif /* _HAKO_SIMEVENT_HPP_ */