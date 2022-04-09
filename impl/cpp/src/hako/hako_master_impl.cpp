#include "hako_master_impl.hpp"
#include "data/hako_master_data.hpp"

bool hako::HakoMasterControllerImpl::execute()
{
    //TODO
    return true;
}

void hako::HakoMasterControllerImpl::set_config_simtime(HakoTimeType max_delay_time_usec, HakoTimeType delta_time_usec)
{
    this->master_data_->lock();
    {
        auto & timeset = this->master_data_->ref_time_nolock();
        timeset.max_delay = max_delay_time_usec;
        timeset.delta = delta_time_usec;
    }
    this->master_data_->unlock();
    return;
}

HakoTimeType hako::HakoMasterControllerImpl::get_max_deltay_time_usec()
{
    auto timeset = this->master_data_->get_time();
    return timeset.max_delay;
}
HakoTimeType hako::HakoMasterControllerImpl::get_delta_time_usec()
{
    auto timeset = this->master_data_->get_time();
    return timeset.delta;
}

int32_t hako::HakoMasterControllerImpl::get_shmid()
{
    return this->master_data_->get_shmid();
}
