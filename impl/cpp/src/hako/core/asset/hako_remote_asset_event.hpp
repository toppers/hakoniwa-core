#ifndef _HAKO_REMOTE_ASSET_EVENT_HPP_
#define _HAKO_REMOTE_ASSET_EVENT_HPP_

#include "data/hako_master_data.hpp"
#include <map>
#include <thread>

namespace hako::core::asset {
    class HakoRemoteAssetEvent {
    public:
        HakoRemoteAssetEvent(std::shared_ptr<data::HakoMasterData> master_data)
        {
            this->master_data_ = master_data;
        }
        virtual ~HakoRemoteAssetEvent() {}
        void start_monitoring(HakoAssetIdType asset_id, AssetCallbackType &callbacks);
        void stop_monitoring(HakoAssetIdType asset_id);

    private:
        HakoRemoteAssetEvent() {}
        void monitor_thread(HakoAssetIdType asset_id, AssetCallbackType *callbacks);
        std::map<HakoAssetIdType, std::shared_ptr<std::thread>> map_;
        std::shared_ptr<data::HakoMasterData> master_data_;
    };
}

#endif /* _HAKO_REMOTE_ASSET_EVENT_HPP_ */
