#ifndef _HAKO_ASSET_IMPL_HPP_
#define _HAKO_ASSET_IMPL_HPP_

#include "hako_asset.hpp"
#include "data/hako_master_data.hpp"
#include "core/asset/hako_remote_asset_event.hpp"

namespace hako {
    class HakoAssetControllerImpl : public IHakoAssetController {
    public:
        HakoAssetControllerImpl(std::shared_ptr<data::HakoMasterData> master_data)
        {
            this->master_data_ = master_data;
            this->remote_event_ = std::make_shared<core::asset::HakoRemoteAssetEvent>(master_data);
        }
        virtual bool asset_register(const std::string & name, AssetCallbackType &callbacks);
        virtual bool asset_remote_register(const std::string & name, AssetCallbackType &callbacks);
        virtual bool asset_unregister(const std::string & name);
        virtual void notify_simtime(const std::string & name, HakoTimeType simtime);
        virtual HakoTimeType get_worldtime();

        /*
         * feedback events
         */
        bool start_feedback(const std::string& asset_name, bool isOk);
        bool stop_feedback(const std::string& asset_name, bool isOk);
        bool reset_feedback(const std::string& asset_name, bool isOk);



    private:
        HakoAssetControllerImpl() {}
        bool feedback(const std::string& asset_name, bool isOk, HakoSimulationStateType exp_state);
        std::shared_ptr<data::HakoMasterData> master_data_;
        std::shared_ptr<core::asset::HakoRemoteAssetEvent> remote_event_;
    };
}

#endif /* _HAKO_ASSET_IMPL_HPP_ */