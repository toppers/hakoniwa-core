#include <iostream>
#include <memory>
#include <string>
#include <mutex>
#include <condition_variable>
#include <atomic>
#include <thread>
#include <queue>

#include <grpcpp/grpcpp.h>

#include "hakoniwa_core.pb.h"
#include "hakoniwa_core.grpc.pb.h"
#include "hakoniwa_client.h"

using grpc::Channel;
using grpc::ClientContext;
using grpc::Status;
using hakoniwa::CoreService;
using hakoniwa::AssetNotification;
using hakoniwa::AssetNotificationReply;
using hakoniwa::NormalReply;
using hakoniwa::AssetInfo;
using hakoniwa::ErrorCode;
using hakoniwa::AssetNotificationEvent;

namespace hakoniwa {

class HakoniwaCoreServiceClient {
 public:
  HakoniwaCoreServiceClient(std::shared_ptr<Channel> channel)
      : stub_(CoreService::NewStub(channel)) {}

  ErcdType Register(const HakoniwaAssetInfoType *asset) {
    AssetInfo request;
    request.set_name(asset->name);
    NormalReply reply;
    ClientContext context;

    Status status = stub_->Register(&context, request, &reply);

    if (status.ok()) {
      if (reply.ercd() == ERROR_CODE_OK) {
        return Ercd_OK;
      }
      else {
        std::cout << "ERROR: " << reply.ercd() << std::endl;
        return Ercd_NG;
      }
    } else {
      std::cout << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }
  ErcdType Unregister(const HakoniwaAssetInfoType *asset) {
    AssetInfo request;
    request.set_name(asset->name);
    NormalReply reply;
    ClientContext context;

    Status status = stub_->Unregister(&context, request, &reply);

    if (status.ok()) {
      if (reply.ercd() == ERROR_CODE_OK) {
        return Ercd_OK;
      }
      else {
        std::cout << "ERROR: " << reply.ercd() << std::endl;
        return Ercd_NG;
      }
    } else {
      std::cout << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }
  ErcdType StartSimulation(void) {
    NormalReply reply;
    const ::google::protobuf::Empty request;
    ClientContext context;

    Status status = stub_->StartSimulation(&context, request, &reply);
    if (status.ok()) {
      if (reply.ercd() != ErrorCode::ERROR_CODE_OK) {
        std::cout << "ERROR: " << reply.ercd() << std::endl;
        return Ercd_NG;
      }
      return Ercd_OK;
    } else {
      std::cout << "ERROR:" << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }
  ErcdType StopSimulation(void) {
    NormalReply reply;
    const ::google::protobuf::Empty request;
    ClientContext context;

    Status status = stub_->StopSimulation(&context, request, &reply);
    if (status.ok()) {
      if (reply.ercd() != ErrorCode::ERROR_CODE_OK) {
        std::cout << "ERROR: " << reply.ercd() << std::endl;
        return Ercd_NG;
      }
      return Ercd_OK;
    } else {
      std::cout << "ERROR:" << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }
  ErcdType ResetSimulation(void) {
    NormalReply reply;
    const ::google::protobuf::Empty request;
    ClientContext context;

    Status status = stub_->ResetSimulation(&context, request, &reply);
    if (status.ok()) {
      if (reply.ercd() != ErrorCode::ERROR_CODE_OK) {
        std::cout << "ERROR: " << reply.ercd() << std::endl;
        return Ercd_NG;
      }
      return Ercd_OK;
    } else {
      std::cout << "ERROR:" << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }
  ErcdType GetSimStatus(SimStatusType *sim_status) {
    SimStatReply reply;
    const ::google::protobuf::Empty request;
    ClientContext context;

    Status status = stub_->GetSimStatus(&context, request, &reply);
    if (status.ok()) {
      if (reply.ercd() != ErrorCode::ERROR_CODE_OK) {
        std::cout << "ERROR: " << reply.ercd() << std::endl;
        return Ercd_NG;
      }
      switch (reply.status()) {
        case SimulationStatus::STATUS_STOPPED:
          *sim_status = SimStatus_Stopped;
          break;
        case SimulationStatus::STATUS_RUNNABLE:
          *sim_status = SimStatus_Runnable;
          break;
        case SimulationStatus::STATUS_RUNNING:
          *sim_status = SimStatus_Running;
          break;
        case SimulationStatus::STATUS_STOPPING:
          *sim_status = SimStatus_Stopping;
          break;
        case SimulationStatus::STATUS_TERMINATED:
        default:
          *sim_status = SimStatus_Terminated;
          break;
      }
      return Ercd_OK;
    } else {
      std::cout << "ERROR:" << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }


  void AsyncAssetNotificationStart()
  {
    notification_is_alive_ = true;
  }

  ErcdType AssetNotificationStart(const HakoniwaAssetInfoType *asset) 
  {
    AssetInfo request;
    AssetNotification notification;
    request.set_name(asset->name);
    ClientContext context;

    std::unique_ptr<grpc::ClientReader<AssetNotification> > reader = stub_->AssetNotificationStart(&context, request);

    while (reader->Read(&notification)) {
        {
            std::unique_lock<std::mutex> lock(mtx_);
            events_.push(new AssetNotification(notification));
            cv_.notify_all();
        }
    }
    {
        std::unique_lock<std::mutex> lock(mtx_);
        notification_is_alive_ = false;
        cv_.notify_all();
    }
    printf("notification end\n");
    Status status = reader->Finish();
    if (status.ok()) {
        return Ercd_OK;
    }
    else {
      return Ercd_NG;
    }
  }
  AssetNotification* GetNotification() 
  {
      std::unique_lock<std::mutex> lock(mtx_);
      if (events_.empty() && (notification_is_alive_ == false)) {
          return nullptr;
      }
      while (events_.empty()) {
        cv_.wait(lock);
        if (notification_is_alive_ == false) {
            return nullptr;
        }
      }
      AssetNotification *ret = events_.front();
      if (ret != nullptr) {
          events_.pop();
      }
      return ret;
  }
  AssetNotificationEvent get_event(HakoniwaAssetEventEnumType event) 
  {
      if (event == HakoniwaAssetEvent_Start) {
          return hakoniwa::ASSET_NOTIFICATION_EVENT_START;
      }
      else if (event == HakoniwaAssetEvent_Heartbeat) {
          return hakoniwa::ASSET_NOTIFICATION_EVENT_HEARTBEAT;
      }
      else if (event == HakoniwaAssetEvent_End) {
          return hakoniwa::ASSET_NOTIFICATION_EVENT_END;
      }
      else {
          return hakoniwa::ASSET_NOTIFICATION_EVENT_NONE;
      }
  }
  ErrorCode get_error_code(ErcdType ercd)
  {
      if (ercd == Ercd_OK) {
          return hakoniwa::ERROR_CODE_OK;
      }
      else {
          return hakoniwa::ERROR_CODE_INVAL;
      }
  }

  ErcdType AssetNotificationFeedback(const HakoniwaAssetInfoType *asset, HakoniwaAssetEventEnumType event, ErcdType ercd) {
    ClientContext context;
    AssetNotificationReply feedback;
    NormalReply reply;
    AssetInfo *info = new AssetInfo();
    info->set_name(asset->name);

    feedback.set_event(get_event(event));
    feedback.set_allocated_asset(info);
    feedback.set_ercd(get_error_code(ercd));

    Status status = stub_->AssetNotificationFeedback(&context, feedback, &reply);
    if (status.ok()) {
      return Ercd_OK;
    } else {
      std::cout << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }
  ErcdType GetAssetList(hakoniwa::AssetInfoList *response) {
    ClientContext context;
    const ::google::protobuf::Empty request;

    Status status = stub_->GetAssetList(&context, request, response);

    if (status.ok()) {
      return Ercd_OK;
    } else {
      std::cout << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }

 private:
  std::unique_ptr<CoreService::Stub> stub_;
  std::queue<AssetNotification*> events_;
  std::mutex mtx_;
  std::condition_variable cv_;
  bool notification_is_alive_ = false;
};

static HakoniwaCoreServiceClient *gl_client;

void hakoniwa_core_init(const char* server)
{
  std::string target_str(server);
  static HakoniwaCoreServiceClient client(grpc::CreateChannel(
      target_str, grpc::InsecureChannelCredentials()));
  gl_client = &client;
  return;
}
ErcdType hakoniwa_core_asset_register(const HakoniwaAssetInfoType* asset)
{
  ErcdType ercd = gl_client->Register(asset);
  std::cout << "Client Register reply received: " << std::endl;
  return ercd;
}

ErcdType hakoniwa_core_asset_unregister(const HakoniwaAssetInfoType* asset)
{
  ErcdType ercd = gl_client->Unregister(asset);
  std::cout << "Client Unregister reply received: " << std::endl;
  return ercd;
}
ErcdType hakoniwa_core_start_simulation(void)
{
  ErcdType ercd = gl_client->StartSimulation();
  return ercd;
}
ErcdType hakoniwa_core_stop_simulation(void)
{
  ErcdType ercd = gl_client->StopSimulation();
  return ercd;
}
ErcdType hakoniwa_core_reset_simulation(void)
{
  ErcdType ercd = gl_client->ResetSimulation();
  return ercd;
}
ErcdType hakoniwa_core_get_simstatus(SimStatusType *status)
{
  ErcdType ercd = gl_client->GetSimStatus(status);
  return ercd;
}

HakoniwaAssetEventType hakoniwa_core_asset_get_event(void)
{
    HakoniwaAssetEventType ev;
    ErcdType ercd;
    ev.type = HakoniwaAssetEvent_None;

    AssetNotification *notification = gl_client->GetNotification();
    if (notification != nullptr) {
        if (notification->event() == hakoniwa::ASSET_NOTIFICATION_EVENT_START) {
            ev.type = HakoniwaAssetEvent_Start;
        }
        else if (notification->event() == hakoniwa::ASSET_NOTIFICATION_EVENT_HEARTBEAT) {
            ev.type = HakoniwaAssetEvent_Heartbeat;
        }
        else if (notification->event() == hakoniwa::ASSET_NOTIFICATION_EVENT_END) {
            ev.type = HakoniwaAssetEvent_End;
        }
        delete notification;
    }
    return ev;
}

static void notification_thread(const HakoniwaAssetInfoType* asset)
{
   std::cout << "####THREAD START: " << std::endl;
   ErcdType ercd = gl_client->AssetNotificationStart(asset);
   std::cout << "Client AssetNotificationStart reply received: " << std::endl;
   std::cout << "####THREAD END: " << std::endl;
   return;
}
ErcdType hakoniwa_core_asset_notification_start(const HakoniwaAssetInfoType* asset)
{
  gl_client->AsyncAssetNotificationStart();
  std::thread thr(notification_thread, asset);
  thr.detach();
  return Ercd_OK;
}

ErcdType hakoniwa_core_asset_event_feedback(const HakoniwaAssetInfoType* asset, HakoniwaAssetEventEnumType event, ErcdType ercd)
{
    return gl_client->AssetNotificationFeedback(asset, event, ercd);
}

ErcdType hakonwia_core_get_asset_list(HakoniwaAssetInfoArrayType *list)
{
  hakoniwa::AssetInfoList response;
  ErcdType ercd = gl_client->GetAssetList(&response);
  int i;
  list->array_size = response.assets_size();
  list->entries = (HakoniwaAssetInfoType *)malloc(sizeof(HakoniwaAssetInfoType) * list->array_size);
  if (list->entries == NULL) {
    return Ercd_NG;
  }
  memset(list->entries, 0, sizeof(HakoniwaAssetInfoType) * list->array_size);
  for (i = 0; i < response.assets_size(); i++) {
    int namelen = strlen(response.assets(i).name().c_str());
    list->entries[i].name = (char*)malloc(namelen + 1);
    if (list->entries[i].name == NULL) {
      goto errdone;
    }
    memcpy((void*)list->entries[i].name, response.assets(i).name().c_str(), namelen);
    list->entries[i].name[namelen] = '\0';
  }

  return Ercd_OK;
errdone:
  hakonwia_core_free_asset_list(list);
  return Ercd_NG;
}
void hakonwia_core_free_asset_list(HakoniwaAssetInfoArrayType *list)
{
  int i;
  if (list->entries == NULL) {
    return;
  }
  for (i = 0; i < list->array_size; i++) {
    if (list->entries[i].name != NULL) {
      free((void*)list->entries[i].name);
      list->entries[i].name = NULL;
    }
  }
  free(list->entries);
  list->entries = NULL;
  return;
}

}
