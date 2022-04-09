#include <sys/shm.h>
#include <sys/stat.h>
#include <sys/ipc.h>
#include<sys/sem.h>

#include "utils/hako_shared_memory.hpp"
#ifdef MACOSX
#else
union semun {
    int val;
    struct semid_ds *buf;
    unsigned short int *array;
    struct seminfo *__buf;
};
#endif

int32_t hako::utils::HakoSharedMemory::create_memory(int32_t id, int32_t size)
{
    int32_t total_size = size + sizeof(SharedMemoryMetaDataType);
    int32_t seg_id = shmget(id, total_size, IPC_CREAT | S_IRUSR | S_IWUSR);
    if (seg_id < 0) {
        printf("ERROR: shmget() id=%d size=%d error=%d\n", id, size, errno);
        return -1;
    }
    void *shared_memory = shmat(seg_id, 0, 0);

    int32_t sem_id = semget(id, 1, 0666 | IPC_CREAT);
    if (sem_id < 0) {
        printf("ERROR: semget() id=%d size=%d error=%d\n", id, size, errno);
        (void)shmdt(shared_memory);
        (void)shmctl (seg_id, IPC_RMID, 0);
        return -1;
    }
    union semun argument;
    unsigned short values[1];
    values[0] = 1;
    argument.array = values;
    int err = semctl(sem_id, 0, SETALL, argument);
    if (err < 0) {
        printf("ERROR: semctl() error = %d segid=%d\n", errno, seg_id);
        (void)shmdt(shared_memory);
        (void)shmctl (seg_id, IPC_RMID, 0);
        (void)semctl(sem_id, 1, IPC_RMID, NULL);
        return -1;
    }
    SharedMemoryMetaDataType *metap = static_cast<SharedMemoryMetaDataType*>(shared_memory);
    metap->sem_id = sem_id;
    metap->data_size = size;
    SharedMemoryInfoType info;
    info.addr = metap;
    info.sem_id = sem_id;
    this->shared_memory_map_.insert(std::make_pair(seg_id, info));
    //printf("segid=%d memp=%p\n", seg_id, this->shared_memory_map_[seg_id].addr);
    return seg_id;
}

void* hako::utils::HakoSharedMemory::load_memory(int32_t seg_id)
{
    void *shared_memory = shmat(seg_id, 0, 0);
    if (shared_memory == nullptr) {
        return nullptr;
    }
    SharedMemoryMetaDataType *metap = static_cast<SharedMemoryMetaDataType*>(shared_memory);
    SharedMemoryInfoType info;
    info.addr = metap;
    info.sem_id = metap->sem_id;
    this->shared_memory_map_.insert(std::make_pair(seg_id, info));
    return &this->shared_memory_map_[seg_id].addr->data[0];
}

void* hako::utils::HakoSharedMemory::lock_memory(int32_t seg_id)
{
    struct sembuf sop;
    sop.sem_num =  0;            // Semaphore number
    sop.sem_op  = -1;            // Semaphore operation is Lock
    sop.sem_flg =  0;            // Operation flag
    int32_t err = semop(this->shared_memory_map_[seg_id].sem_id, &sop, 1);
    if (err < 0) {
        printf("ERROR: unlock_memory() semop() error=%d segid=%d\n", errno, seg_id);
    }
    return &this->shared_memory_map_[seg_id].addr->data[0];
}

void hako::utils::HakoSharedMemory::unlock_memory(int32_t seg_id)
{ 
    struct sembuf sop;
    sop.sem_num =  0;            // Semaphore number
    sop.sem_op  =  1;            // Semaphore operation is Lock
    sop.sem_flg =  0;            // Operation flag
    int32_t err = semop(this->shared_memory_map_[seg_id].sem_id, &sop, 1);
    if (err < 0) {
        printf("ERROR: unlock_memory() semop() error=%d segid=%d\n", errno, seg_id);
    }
    return;
}

void hako::utils::HakoSharedMemory::destroy_memory(int32_t seg_id)
{
    void *addr = this->shared_memory_map_[seg_id].addr;
    if (addr != nullptr) {
        (void)shmdt(addr);
        (void)shmctl (seg_id, IPC_RMID, 0);
        (void)semctl(this->shared_memory_map_[seg_id].sem_id, 1, IPC_RMID, NULL);
        this->shared_memory_map_.erase(seg_id);
    }
    return;
}