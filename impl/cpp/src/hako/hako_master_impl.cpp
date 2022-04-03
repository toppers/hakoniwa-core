#include "hako_master_impl.hpp"

bool hako::HakoMasterControllerImpl::execute()
{
    //TODO
    return true;
}

void hako::HakoMasterControllerImpl::set_config_simtime(HakoTimeType max_delay_time, HakoTimeType delta_time)
{
    this->max_delay_time_ = max_delay_time;
    this->delta_time_ = delta_time;
    return;
}