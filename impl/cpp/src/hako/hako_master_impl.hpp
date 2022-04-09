#ifndef _HAKO_MASTER_IMPL_HPP_
#define _HAKO_MASTER_IMPL_HPP_

#include "hako_master.hpp"
#include "data/hako_master_data.hpp"

namespace hako {
    class HakoMasterControllerImpl : public IHakoMasterController {
    public:
        HakoMasterControllerImpl(std::shared_ptr<data::HakoMasterData> ptr)
        {
            this->master_data_ = ptr;
        }
        virtual bool execute();
        virtual void set_config_simtime(HakoTimeType max_delay_time_usec, HakoTimeType delta_time_usec);
        virtual HakoTimeType get_max_deltay_time_usec();
        virtual HakoTimeType get_delta_time_usec();
        virtual int32_t get_shmid();
    private:
        HakoMasterControllerImpl() {}
        std::shared_ptr<data::HakoMasterData> master_data_;
    };
}

#endif /* _HAKO_MASTER_IMPL_HPP_ */