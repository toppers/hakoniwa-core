#ifndef _HAKO_BASE_DATA_HPP_
#define _HAKO_BASE_DATA_HPP_

#include "types/hako_types.hpp"

namespace hako::data {

    typedef struct {
        HakoTimeType    max_delay;
        HakoTimeType    delta;
        HakoTimeType    current;
    } HakoTimeSetType;

    typedef enum {
        Unknown = 0,
        Inside,
        Outside,
        Count
    } HakoAssetType;

    typedef struct {
        HakoAssetIdType     id;
        HakoFixedStringType name;
        HakoAssetType       type;
        HakoTimeType        ctime;
        AssetCallbackType   callback;
    } HakoAssetEntryType;

}

#endif /* _HAKO_BASE_DATA_HPP_ */