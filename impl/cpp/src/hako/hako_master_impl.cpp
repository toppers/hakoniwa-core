#include "hako_master_impl.hpp"

bool hako::HakoMasterControllerImpl::execute()
{
    //TODO
    return true;
}

void hako::HakoMasterControllerImpl::set_config_simtime(HakoTimeType max_delay_time_usec, HakoTimeType delta_time_usec)
{
    this->hako_time_set_.max_delay_time_usec = max_delay_time_usec;
    this->hako_time_set_.delta_time_usec = delta_time_usec;
    this->hako_time_set_.current_time_usec = 0ULL;
    return;
}
