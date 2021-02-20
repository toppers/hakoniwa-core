#include <iostream>
#include <memory>
#include <string>

#include <grpcpp/grpcpp.h>

#include "hakoniwa_core.grpc.pb.h"
#include "hakoniwa_client.h"

using grpc::Channel;
using grpc::ClientContext;
using grpc::Status;
using hakoniwa::HakoniwaCoreService;
using hakoniwa::HakoniwaReply;
using hakoniwa::HakoniwaAssetInfo;
using hakoniwa::HakoniwaCoreService;

class HakoniwaCoreServiceClient {
 public:
  HakoniwaCoreServiceClient(std::shared_ptr<Channel> channel)
      : stub_(HakoniwaCoreService::NewStub(channel)) {}

  ErcdType Register(const HakoniwaAssetInfoType *asset) {
    HakoniwaAssetInfo request;
    request.set_name(asset->name);
    HakoniwaReply reply;
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
    HakoniwaAssetInfo request;
    request.set_name(asset->name);
    HakoniwaReply reply;
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

 private:
  std::unique_ptr<HakoniwaCoreService::Stub> stub_;
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

