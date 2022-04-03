#ifndef _HAKO_ASSET_HPP_
#define _HAKO_ASSET_HPP_

#include <string>
#include "types/hako_types.hpp"

namespace hako {

    class IHakoAssetController {
    public:
        virtual ~IHakoAssetController() {}

        /*
         * Asset APIs
         */
        virtual bool asset_register(const std::string & name, AssetCallbackType &callbacks) = 0;
        virtual bool asset_unregister(const std::string & name) = 0;
        virtual void notify_simtime(HakoTimeType simtime) = 0;
        virtual HakoTimeType get_worldtime() = 0;

    };
}

#endif /* _HAKO_ASSET_HPP_ */