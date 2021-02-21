#ifndef _HAKONIW_CLIENT_H_
#define _HAKONIW_CLIENT_H_

#ifdef __cplusplus
extern "C" {
#endif

extern void hakoniwa_core_init(const char* server);
typedef enum {
    Ercd_OK = 0,
    Ercd_NG
} ErcdType;
typedef struct {
    const char *name;
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
extern HakoniwaAssetEventType hakoniwa_core_asset_get_event(void);

#ifdef __cplusplus
}
#endif

#endif /* _HAKONIW_CLIENT_H_ */