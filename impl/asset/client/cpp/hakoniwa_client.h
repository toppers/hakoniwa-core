#ifndef _HAKONIW_CLIENT_H_
#define _HAKONIW_CLIENT_H_

namespace hakoniwa {

#ifdef __cplusplus
extern "C" {
#endif

extern void hakoniwa_core_init(const char* server);
typedef enum {
    Ercd_OK = 0,
    Ercd_NG
} ErcdType;
typedef struct {
    char *name;
    int len;
} HakoniwaAssetInfoType;
extern ErcdType hakoniwa_core_asset_register(const HakoniwaAssetInfoType* asset);
extern ErcdType hakoniwa_core_asset_unregister(const HakoniwaAssetInfoType* asset);

typedef enum {
    HakoniwaAssetEvent_Start = 0,
    HakoniwaAssetEvent_End,
    HakoniwaAssetEvent_None,
} HakoniwaAssetEventEnumType;

typedef struct {
    HakoniwaAssetEventEnumType type;
} HakoniwaAssetEventType;
extern ErcdType hakoniwa_core_asset_notification_start(const HakoniwaAssetInfoType* asset);
extern HakoniwaAssetEventType hakoniwa_core_asset_get_event(void);
extern ErcdType hakoniwa_core_asset_event_feedback(const HakoniwaAssetInfoType* asset, HakoniwaAssetEventEnumType event, ErcdType ercd);

typedef struct {
    int array_size;
    HakoniwaAssetInfoType *entries;
} HakoniwaAssetInfoArrayType;
extern ErcdType hakonwia_core_get_asset_list(HakoniwaAssetInfoArrayType *list);
extern void hakonwia_core_free_asset_list(HakoniwaAssetInfoArrayType *list);

#ifdef __cplusplus
}
#endif

}

#endif /* _HAKONIW_CLIENT_H_ */