#include "hakoniwa_client.h"
#include <fstream>
#include <iostream>
#include <string>
#include <string.h>
#include <stdio.h>
#include <unistd.h>
#include "hakoniwa_process.h"
#include "nlohmann/json.hpp"
using json = nlohmann::json;

using namespace hakoniwa;

typedef struct {
  json param;
  ProcessManager process;
  HakoniwaAssetInfoType *asset;
} HakoniwaProxyControllerType;


static ErcdType init(char *ip_port, HakoniwaProxyControllerType *ctrlp)
{
  ErcdType err;

  hakoniwa_core_init(ip_port);

  err = hakoniwa_core_asset_register(ctrlp->asset);
  if (err != Ercd_OK) {
    printf("ERROR: hakoniwa_core_asset_register() returns %d\n", err);
    return err;
  }
  printf("INFO: Register Asset %s success\n", ctrlp->asset->name);
  err = hakoniwa_core_asset_notification_start(ctrlp->asset);
  if (err != Ercd_OK) {
    printf("hakoniwa_core_asset_notification_start() returns %d\n", err);
    return err;
  }
  printf("INFO: Notification Setting success\n");
  ctrlp->process.set_current_dir(ctrlp->param["target_exec_dir"]);
  ctrlp->process.set_binary_path(ctrlp->param["target_bin_path"]);
  for (int i = 0; i < ctrlp->param["target_options"].size(); i++) {
    ctrlp->process.add_option(ctrlp->param["target_options"][i]);
  }
  return Ercd_OK;
}

int main(int argc, char** argv) 
{
  HakoniwaProxyControllerType ctrl;
  char ip_port[128];
  char *param_file = nullptr;

  if (argc != 4) {
    printf("Usage: %s <param_file> <ipaddr> <portno>\n", argv[0]);
    return 1;
  }
  HakoniwaAssetInfoType asset;
  ctrl.asset = &asset;
  param_file = argv[1];
  std::ifstream ifs(param_file);
  ctrl.param = json::parse(ifs);
  asset.name = (char*)ctrl.param["asset_name"].get<std::string>().c_str();
  asset.len = strlen(ctrl.param["asset_name"].get<std::string>().c_str());

  sprintf(ip_port, "%s:%s", argv[2], argv[3]);
  ErcdType err = init(ip_port, &ctrl);
  if (err != Ercd_OK) {
    return 1;
  }

  while (true) {
    HakoniwaAssetEventType ev = hakoniwa_core_asset_get_event();
    if (ev.type == HakoniwaAssetEvent_None) {
        break;
    }
    bool result = false;
    switch (ev.type) {
      case HakoniwaAssetEvent_Start:
        result = ctrl.process.invoke();
        break;
      case HakoniwaAssetEvent_End:
        result = true;
        ctrl.process.terminate();
       break;
      case HakoniwaAssetEvent_Heartbeat:
        result = true;
        break;
      default:
        break;
    }
    if (result) {
      hakoniwa_core_asset_event_feedback(ctrl.asset, ev.type, Ercd_OK);
    }
    else {
      hakoniwa_core_asset_event_feedback(ctrl.asset, ev.type, Ercd_NG);
    }
  }
  err = hakoniwa_core_asset_unregister(ctrl.asset);
  printf("INFO Unregister Asset %s result=%d\n", ctrl.asset->name, err);
  return 0;
}