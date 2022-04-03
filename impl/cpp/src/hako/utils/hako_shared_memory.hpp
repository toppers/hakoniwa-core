#ifndef _HAKO_SHARED_MEMORY_HPP_
#define _HAKO_SHARED_MEMORY_HPP_

#include "types/hako_types.hpp"
#include <map>

namespace hako::utils {
    typedef struct {
        void *addr;
        int32_t sem_id;
    } SharedMemoryInfoType;
    class HakoSharedMemory {
    public:
        HakoSharedMemory() {}
        virtual ~HakoSharedMemory() {}

        int32_t create_memory(int32_t id, int32_t size);
        void* lock_memory(int32_t seg_id);
        void unlock_memory(int32_t seg_id);
        void destroy_memory(int32_t id);

    private:
        std::map<int32_t, SharedMemoryInfoType> shared_memory_map_;
    };
}


#endif /* _HAKO_SHARED_MEMORY_HPP_ */