#ifndef _HAKO_ASSET_IMPL_HPP_
#define _HAKO_ASSET_IMPL_HPP_

#include "hako_asset.hpp"
#include "data/hako_master_data.hpp"

namespace hako {
    class HakoAssetControllerImpl : public IHakoAssetController {
    public:
        HakoAssetControllerImpl(std::shared_ptr<data::HakoMasterData> master_data)
        {
            this->master_data_ = master_data;
        }
        virtual bool asset_register(const std::string & name, AssetCallbackType &callbacks);
        virtual bool asset_remote_register(const std::string & name, AssetCallbackType &callbacks);
        virtual bool asset_unregister(const std::string & name);
        virtual void notify_simtime(const std::string & name, HakoTimeType simtime);
        virtual HakoTimeType get_worldtime();
    private:
        HakoAssetControllerImpl() {}
        std::shared_ptr<data::HakoMasterData> master_data_;
    };
}

#endif /* _HAKO_ASSET_IMPL_HPP_ */