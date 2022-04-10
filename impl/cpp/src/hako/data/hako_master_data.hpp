#ifndef _HAKO_MASTER_DATA_HPP_
#define _HAKO_MASTER_DATA_HPP_

#include "data/hako_base_data.hpp"
#include "utils/hako_shared_memory.hpp"
#include "utils/hako_string.hpp"
#include <string.h>

namespace hako::data {

    typedef struct {
        HakoSimulationStateType state;
        HakoTimeSetType         time_usec;
        uint32_t                asset_num;
        HakoAssetEntryType      assets[HAKO_DATA_MAX_ASSET_NUM];
        HakoAssetEntryEventType assets_ev[HAKO_DATA_MAX_ASSET_NUM];
    } HakoMasterDataType;

    class HakoMasterData {
    public:
        HakoMasterData()
        {
        }
        virtual ~HakoMasterData()
        {
        }
        void init()
        {
            this->shmp_ = std::make_shared<hako::utils::HakoSharedMemory>();
            auto shmid = this->shmp_->create_memory(HAKO_SHARED_MEMORY_ID_0, sizeof(HakoMasterDataType));
            if (shmid < 0) {
                throw std::bad_alloc();
            }
            void *datap = this->shmp_->lock_memory(HAKO_SHARED_MEMORY_ID_0);
            this->master_datap_ = static_cast<HakoMasterDataType*>(datap);
            {
                memset(this->master_datap_, 0, sizeof(HakoMasterDataType));
            }
            this->shmp_->unlock_memory(HAKO_SHARED_MEMORY_ID_0);
        }

        void load()
        {
            if (this->shmp_ != nullptr) {
                return;
            }
            this->shmp_ = std::make_shared<hako::utils::HakoSharedMemory>();
            void *datap = this->shmp_->load_memory(HAKO_SHARED_MEMORY_ID_0, sizeof(HakoMasterDataType));
            this->master_datap_ = static_cast<HakoMasterDataType*>(datap);
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            return;
        }

        void destroy()
        {
            if (this->shmp_ != nullptr) {
                this->shmp_->destroy_memory(HAKO_SHARED_MEMORY_ID_0);
                this->master_datap_ = nullptr;
            }
        }

        /*
         * data exclusive lock APIs
         */
        void lock()
        {
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            (void)this->shmp_->lock_memory(HAKO_SHARED_MEMORY_ID_0);
        }
        void unlock()
        {
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            (void)this->shmp_->unlock_memory(HAKO_SHARED_MEMORY_ID_0);
        }
        /*
         * Time APIs
         */
        HakoTimeSetType get_time()
        {
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            this->lock();
            HakoTimeSetType timeset = this->master_datap_->time_usec;
            this->unlock();
            return timeset;
        }
        HakoTimeSetType& ref_time_nolock()
        {
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            HakoTimeSetType &timeset = this->master_datap_->time_usec;
            return timeset;
        }
        HakoSimulationStateType& ref_state_nolock()
        {
            return this->master_datap_->state;
        }
        void publish_event_nolock(HakoAssetEventType event)
        {
            for (int i = 0; i < HAKO_DATA_MAX_ASSET_NUM; i++) {
                auto& entry = this->master_datap_->assets[i];
                auto& entry_ev = this->master_datap_->assets_ev[i];
                if (entry.type != hako::data::HakoAssetType::HakoAsset_Unknown) {
                    entry_ev.event = event;
                    entry_ev.event_feedback = false;
                    switch (event) {
                        case hako::data::HakoAssetEvent_Start:
                           entry.callback.start();
                            break;
                        case hako::data::HakoAssetEvent_Stop:
                           entry.callback.stop();
                            break;
                        case hako::data::HakoAssetEvent_Reset:
                           entry.callback.reset();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        /*
         * Assets APIs
         */        
        HakoAssetIdType alloc_asset(const std::string &name, HakoAssetType type, AssetCallbackType &callback)
        {
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            if (type == hako::data::HakoAssetType::HakoAsset_Unknown) {
                return -1;
            }
            if (name.length() > HAKO_FIXED_STRLEN_MAX) {
                return -1;
            }
            HakoAssetIdType id = -1;
            this->lock();
            if (this->master_datap_->state == HakoSim_Stopped &&
                (this->get_asset_nolock(name) == nullptr) &&
                (this->master_datap_->asset_num < HAKO_DATA_MAX_ASSET_NUM)) {
                for (int i = 0; i < HAKO_DATA_MAX_ASSET_NUM; i++) {
                    if (this->master_datap_->assets[i].type == hako::data::HakoAssetType::HakoAsset_Unknown) {
                        this->master_datap_->assets[i].id = i;
                        this->master_datap_->assets[i].type = type;
                        this->master_datap_->assets[i].callback = callback;
                        this->master_datap_->assets_ev[i].ctime = 0ULL;
                        hako::utils::hako_string2fixed(name, this->master_datap_->assets[i].name);
                        id = i;
                        this->master_datap_->asset_num++;
                        break;
                    }
                }
            }
            else {
                /* nothing to do */
            }
            this->unlock();
            return id;
        }
        bool free_asset(const std::string &name)
        {
            bool ret = false;
            this->lock();
            if (this->master_datap_->state == HakoSim_Stopped) {
                HakoAssetEntryType *entry = this->get_asset_nolock(name);
                if (entry != nullptr) {
                    entry->type = hako::data::HakoAssetType::HakoAsset_Unknown;
                    this->master_datap_->asset_num--;
                    ret = true;
                }
            }
            this->unlock();
            return ret;
        }
        HakoAssetEntryType *get_asset(HakoAssetIdType id)
        {
            if ((id >= 0) && (id < HAKO_DATA_MAX_ASSET_NUM)) {
                if (this->master_datap_->assets[id].type != hako::data::HakoAssetType::HakoAsset_Unknown) {
                    return &this->master_datap_->assets[id];
                }
            }
            return nullptr;
        }
        HakoAssetEntryEventType *get_asset_event(HakoAssetIdType id)
        {
            if ((id >= 0) && (id < HAKO_DATA_MAX_ASSET_NUM)) {
                if (this->master_datap_->assets[id].type != hako::data::HakoAssetType::HakoAsset_Unknown) {
                    return &this->master_datap_->assets_ev[id];
                }
            }
            return nullptr;
        }

        HakoAssetEntryType *get_asset(const std::string &name)
        {
            HakoAssetEntryType *entry;
            this->lock();
            entry = this->get_asset_nolock(name);
            this->unlock();
            return entry;
        }
        bool is_all_feedback_nolock()
        {
            bool ret = true;
            for (int i = 0; i < HAKO_DATA_MAX_ASSET_NUM; i++) {
                HakoAssetEntryType &entry = this->master_datap_->assets[i];
                if (entry.type == hako::data::HakoAssetType::HakoAsset_Unknown) {
                    continue;
                }
                else if (this->master_datap_->assets_ev[i].event_feedback == false) {
                    ret = false;
                    break;
                }
            }
            return ret;
        }
    private:
        HakoAssetEntryType *get_asset_nolock(const std::string &name)
        {
            for (int i = 0; i < HAKO_DATA_MAX_ASSET_NUM; i++) {
                HakoAssetEntryType &entry = this->master_datap_->assets[i];
                if (entry.type == hako::data::HakoAssetType::HakoAsset_Unknown) {
                    continue;
                }
                else if (entry.name.len != name.length()) {
                    continue;
                }
                else if (strncmp(entry.name.data, name.c_str(), entry.name.len) != 0) {
                    continue;
                }
                return &entry;
            }
            return nullptr;
        }

        std::shared_ptr<hako::utils::HakoSharedMemory>  shmp_;
        HakoMasterDataType *master_datap_ = nullptr;
    };
}

#endif /* _HAKO_MASTER_DATA_HPP_ */