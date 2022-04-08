#ifndef _HAKO_ASSET_IMPL_HPP_
#define _HAKO_ASSET_IMPL_HPP_

#include "hako_asset.hpp"

namespace hako {
    typedef enum {
        Unknown = 0,
        Inside,
        Outside,
        Count
    } HakoAssetType;
    typedef struct {
        HakoAssetIdType     id;
        HakoAssetNameType   name;
        HakoAssetType       type;
        HakoTimeType        ctime;
    } HakoAssetEntryType;
    class HakoAssetControllerImpl : public IHakoAssetController {
    public:
        HakoAssetControllerImpl() {}
        virtual bool asset_register(const std::string & name, AssetCallbackType &callbacks);
        virtual bool asset_unregister(const std::string & name);
        virtual void notify_simtime(HakoTimeType simtime);
        virtual HakoTimeType get_worldtime() = 0;
    private:
    };
}

#endif /* _HAKO_ASSET_IMPL_HPP_ */