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
        HakoAsset_Unknown = 0,
        HakoAsset_Inside,
        HakoAsset_Outside,
        HakoAsset_Count
    } HakoAssetType;


    typedef struct {
        HakoAssetIdType     id;
        HakoFixedStringType name;
        HakoAssetType       type;
        AssetCallbackType   callback;
    } HakoAssetEntryType;

    typedef enum {
        HakoAssetEvent_None = 0,
        HakoAssetEvent_Start,
        HakoAssetEvent_Stop,
        HakoAssetEvent_Reset,
        HakoAssetEvent_Count
    } HakoAssetEventType;

    typedef struct {
        HakoTimeType        ctime;
        HakoAssetEventType  event;
        bool                event_feedback;
    } HakoAssetEntryEventType;
}

#endif /* _HAKO_BASE_DATA_HPP_ */