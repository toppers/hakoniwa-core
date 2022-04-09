#ifndef _HAKO_MASTER_DATA_HPP_
#define _HAKO_MASTER_DATA_HPP_

#include "data/hako_base_data.hpp"
#include "utils/hako_shared_memory.hpp"
#include "utils/hako_string.hpp"
#include <string.h>

namespace hako::data {

    typedef struct {
        HakoTimeSetType     time_usec;
        uint32_t            asset_num;
        HakoAssetEntryType  assets[HAKO_DATA_MAX_ASSET_NUM];
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
            this->seg_id_ = this->shmp_->create_memory(HAKO_SHARED_MEMORY_ID_0, sizeof(HakoMasterDataType));
            if (this->seg_id_ < 0) {
                throw std::bad_alloc();
            }
            void *datap = this->shmp_->lock_memory(this->seg_id_);
            this->master_datap_ = static_cast<HakoMasterDataType*>(datap);
            {
                memset(this->master_datap_, 0, sizeof(HakoMasterDataType));
            }
            this->shmp_->unlock_memory(this->seg_id_);
        }
        void load(int32_t seg_id)
        {
            if (this->shmp_ != nullptr) {
                return;
            }
            this->shmp_ = std::make_shared<hako::utils::HakoSharedMemory>();
            void *datap = this->shmp_->load_memory(seg_id);
            this->master_datap_ = static_cast<HakoMasterDataType*>(datap);
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            this->seg_id_ = seg_id;
            return;
        }
        void destroy()
        {
            if (this->shmp_ != nullptr) {
                this->shmp_->destroy_memory(this->seg_id_);
                this->seg_id_ = -1;
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
            (void)this->shmp_->lock_memory(this->seg_id_);
        }
        void unlock()
        {
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            (void)this->shmp_->unlock_memory(this->seg_id_);
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
        /*
         * Assets APIs
         */        
        HakoAssetIdType alloc_asset(const std::string &name, HakoAssetType type)
        {
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            if (type == hako::data::HakoAssetType::Unknown) {
                return -1;
            }
            if (name.length() > HAKO_FIXED_STRLEN_MAX) {
                return -1;
            }
            HakoAssetIdType id = -1;
            this->lock();
            if ((this->get_asset_nolock(name) == nullptr) &&
                (this->master_datap_->asset_num < HAKO_DATA_MAX_ASSET_NUM)) {
                for (int i = 0; i < HAKO_DATA_MAX_ASSET_NUM; i++) {
                    if (this->master_datap_->assets[i].type == hako::data::HakoAssetType::Unknown) {
                        this->master_datap_->assets[i].id = i;
                        this->master_datap_->assets[i].type = type;
                        this->master_datap_->assets[i].ctime = 0ULL;
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
        void free_asset(const std::string &name)
        {
            this->lock();
            HakoAssetEntryType *entry = this->get_asset_nolock(name);
            if (entry != nullptr) {
                entry->type = hako::data::HakoAssetType::Unknown;
                this->master_datap_->asset_num--;
            }
            this->unlock();
        }
        HakoAssetEntryType *get_asset(HakoAssetIdType id)
        {
            if ((id >= 0) && (id >= HAKO_DATA_MAX_ASSET_NUM)) {
                if (this->master_datap_->assets[id].type != hako::data::HakoAssetType::Unknown) {
                    return &this->master_datap_->assets[id];
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

    private:
        HakoAssetEntryType *get_asset_nolock(const std::string &name)
        {
            for (int i = 0; i < HAKO_DATA_MAX_ASSET_NUM; i++) {
                HakoAssetEntryType &entry = this->master_datap_->assets[i];
                if (entry.type == hako::data::HakoAssetType::Unknown) {
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
        int32_t seg_id_ = -1;
        HakoMasterDataType *master_datap_ = nullptr;
    };
}

#endif /* _HAKO_MASTER_DATA_HPP_ */