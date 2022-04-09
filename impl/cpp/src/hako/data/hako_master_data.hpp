#ifndef _HAKO_MASTER_DATA_HPP_
#define _HAKO_MASTER_DATA_HPP_

#include "data/hako_base_data.hpp"
#include "utils/hako_shared_memory.hpp"
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
        HakoTimeSetType &get_time()
        {
            if ((this->shmp_ == nullptr) || (this->master_datap_ == nullptr)) {
                throw std::invalid_argument("ERROR: not initialized yet");
            }
            return this->master_datap_->time_usec;
        }
        /*
         * Assets APIs
         */        
        bool alloc_asset(const std::string &name);
        void free_asset(const std::string &name);
        HakoAssetEntryType &get_asset(const std::string &name);

    private:
        std::shared_ptr<hako::utils::HakoSharedMemory>  shmp_;
        int32_t seg_id_ = 01;
        HakoMasterDataType *master_datap_;
    };
}

#endif /* _HAKO_MASTER_DATA_HPP_ */