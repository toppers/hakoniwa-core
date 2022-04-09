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
        auto timeset = this->master_data_->get_time();
        timeset->max_delay = max_delay_time_usec;
        timeset->delta = delta_time_usec;
    }
    this->master_data_->unlock();
    return;
}

HakoTimeType hako::HakoMasterControllerImpl::get_max_deltay_time_usec()
{
    HakoTimeType max_delay;
    this->master_data_->lock();
    {
        hako::data::HakoTimeSetType* timeset = this->master_data_->get_time();
        max_delay = timeset->max_delay;
    }
    this->master_data_->unlock();
    return max_delay;
}
HakoTimeType hako::HakoMasterControllerImpl::get_delta_time_usec()
{
    HakoTimeType delta;
    this->master_data_->lock();
    {
        hako::data::HakoTimeSetType* timeset = this->master_data_->get_time();
        delta = timeset->delta;
    }
    this->master_data_->unlock();
    return delta;
}

