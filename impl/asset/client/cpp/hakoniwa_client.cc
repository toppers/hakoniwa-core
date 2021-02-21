#include <iostream>
#include <memory>
#include <string>
#include <mutex>
#include <condition_variable>
#include <atomic>
#include <thread>
#include <queue>

#include <grpcpp/grpcpp.h>

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
      return Ercd_OK;
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
      return Ercd_OK;
    } else {
      std::cout << status.error_code() << ": " << status.error_message()
                << std::endl;
      return Ercd_NG;
    }
  }
  void AssetNotificationStart()
  {
    notification_is_alive_ = true;
  }
  ErcdType AssetNotificationStart(const HakoniwaAssetInfoType *asset) {
    AssetInfo request;
    AssetNotification notification;
    request.set_name(asset->name);
    ClientContext context;

    std::unique_ptr<grpc::ClientReader<AssetNotification> > reader = stub_->AssetNotificationStart(&context, request);

    while (reader->Read(&notification)) {
        std::cout << "Found notification  "
                    << notification.event()  << std::endl;
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
  AssetNotification* GetNotification() {
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
  ErcdType AssetNotificationFeedback(AssetInfo *asset, AssetNotificationEvent event, ErrorCode ercd) {
    ClientContext context;
    AssetNotificationReply feedback;
    NormalReply reply;

    feedback.set_event(event);
    feedback.set_allocated_asset(asset);
    feedback.set_ercd(ercd);

    Status status = stub_->AssetNotificationFeedback(&context, feedback, &reply);

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
  gl_client->AssetNotificationStart();
  std::thread thr(notification_thread, asset);
  thr.detach();
  
  return Ercd_OK;
}