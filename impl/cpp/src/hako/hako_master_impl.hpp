#ifndef _HAKO_MASTER_IMPL_HPP_
#define _HAKO_MASTER_IMPL_HPP_

#include "hako_master.hpp"

namespace hako {
    typedef struct {
        HakoTimeType    max_delay_time_usec;
        HakoTimeType    delta_time_usec;
        HakoTimeType    current_time_usec;
    } HakoTimeSetType;
    class HakoMasterControllerImpl : public IHakoMasterController {
    public:
        HakoMasterControllerImpl() {}
        virtual bool execute();
        virtual void set_config_simtime(HakoTimeType max_delay_time_usec, HakoTimeType delta_time_usec);
        virtual HakoTimeType get_max_deltay_time_usec()
        {
            return this->hako_time_set_.max_delay_time_usec;
        }
        virtual HakoTimeType get_delta_time_usec()
        {
            return this->hako_time_set_.delta_time_usec;
        }
    private:
        HakoTimeSetType hako_time_set_;
    };
}

#endif /* _HAKO_MASTER_IMPL_HPP_ */