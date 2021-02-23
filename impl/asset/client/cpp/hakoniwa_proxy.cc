#include "hakoniwa_client.h"
#include <string.h>
#include <stdio.h>
#include <unistd.h>
#include "hakoniwa_process.h"

using namespace hakoniwa;

int main(int argc, char** argv) 
{
  char buf[128];
  int retlen;
  char ip_port[128];

  if (argc != 3) {
    printf("Usage: %s <ipaddr> <portno>\n", argv[0]);
    return 1;
  }
  sprintf(ip_port, "%s:%s", argv[1], argv[2]);
  hakoniwa_core_init(ip_port);

  ErcdType err;
  HakoniwaAssetInfoType asset;

  asset.name = (char*)"Athrill";
  asset.len = strlen(asset.name);
  err = hakoniwa_core_asset_register(&asset);
  printf("hakoniwa_core_asset_register() returns %d\n", err);

  HakoniwaAssetInfoArrayType list;
  err = hakonwia_core_get_asset_list(&list);
  printf("hakonwia_core_get_asset_list() returns %d\n", err);
  int i;
  for (i = 0; i < list.array_size; i++) {
    printf("entry[%d]=%s\n", i, list.entries[i].name);
  }
  hakonwia_core_free_asset_list(&list);

  err = hakoniwa_core_asset_notification_start(&asset);
  printf("hakoniwa_core_asset_notification_start() returns %d\n", err);

  ProcessManager process;

  process.set_current_dir("../cpp");
  process.set_binary_path("./start_athrill.bash");
  process.add_option("base_practice_1");

  process.invoke();
  usleep(1000*1000 * 5);

  process.terminate();

  while (true) {
    HakoniwaAssetEventType ev = hakoniwa_core_asset_get_event();
    printf("hakoniwa_core_asset_get_event() returns %d\n", ev.type);
    if (ev.type == HakoniwaAssetEvent_None) {
        break;
    }
  }
  hakoniwa_core_asset_event_feedback(&asset, HakoniwaAssetEvent_End, Ercd_OK);

  err = hakoniwa_core_asset_unregister(&asset);
  printf("hakoniwa_core_asset_unregister() returns %d\n", err);
  return 0;
}