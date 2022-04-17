#include <sys/shm.h>
#include <sys/stat.h>
#include <sys/ipc.h>
#include<sys/sem.h>

#include "utils/hako_shared_memory.hpp"

int32_t hako::utils::HakoSharedMemory::create_memory(int32_t key, int32_t size)
{
    int32_t total_size = size + sizeof(SharedMemoryMetaDataType);
    int32_t shmid = shmget(key, total_size, IPC_CREAT | S_IRUSR | S_IWUSR);
    if (shmid < 0) {
        printf("ERROR: shmget() id=%d size=%d error=%d\n", key, size, errno);
        return -1;
    }
    void *shared_memory = shmat(shmid, 0, 0);

    int32_t sem_id = semget(key, 1, 0666 | IPC_CREAT);
    if (sem_id < 0) {
        printf("ERROR: semget() key=%d size=%d error=%d\n", key, size, errno);
        (void)shmdt(shared_memory);
        (void)shmctl (shmid, IPC_RMID, 0);
        return -1;
    }

    union semun argument;
    unsigned short values[1];
    values[0] = 1;
    argument.array = values;
    int err = semctl(sem_id, 0, SETALL, argument);
    if (err < 0) {
        printf("ERROR: semctl() error = %d segid=%d\n", errno, shmid);
        (void)shmdt(shared_memory);
        (void)shmctl (shmid, IPC_RMID, 0);
        (void)semctl(sem_id, 1, IPC_RMID, NULL);
        return -1;
    }
    SharedMemoryMetaDataType *metap = static_cast<SharedMemoryMetaDataType*>(shared_memory);
    metap->sem_id = sem_id;
    metap->shm_id = shmid;
    metap->data_size = size;
    SharedMemoryInfoType info;
    info.addr = metap;
    info.shm_id = shmid;
    info.sem_id = sem_id;
    this->shared_memory_map_.insert(std::make_pair(key, info));
    return shmid;
}
void* hako::utils::HakoSharedMemory::load_memory(int32_t key, int32_t size)
{
    int32_t total_size = size + sizeof(SharedMemoryMetaDataType);
    int32_t shmid = shmget(key, total_size, S_IRUSR | S_IWUSR);
    if (shmid < 0) {
        printf("ERROR: shmget() key=%d size=%d error=%d\n", key, size, errno);
        return nullptr;
    }
    return this->load_memory_shmid(key, shmid);
}

void* hako::utils::HakoSharedMemory::load_memory_shmid(int32_t key, int32_t shmid)
{
    void *shared_memory = shmat(shmid, 0, 0);
    if (shared_memory == nullptr) {
        return nullptr;
    }
    SharedMemoryMetaDataType *metap = static_cast<SharedMemoryMetaDataType*>(shared_memory);
    SharedMemoryInfoType info;
    info.addr = metap;
    info.shm_id = metap->shm_id;
    info.sem_id = metap->sem_id;
    this->shared_memory_map_.insert(std::make_pair(key, info));
    return &this->shared_memory_map_[key].addr->data[0];
}

void* hako::utils::HakoSharedMemory::lock_memory(int32_t key)
{
    struct sembuf sop;
    sop.sem_num =  0;            // Semaphore number
    sop.sem_op  = -1;            // Semaphore operation is Lock
    sop.sem_flg =  0;            // Operation flag
    int32_t err = semop(this->shared_memory_map_[key].sem_id, &sop, 1);
    if (err < 0) {
        printf("ERROR: unlock_memory() semop() error=%d key=%d\n", errno, key);
    }
    return &this->shared_memory_map_[key].addr->data[0];
}

void hako::utils::HakoSharedMemory::unlock_memory(int32_t key)
{ 
    struct sembuf sop;
    sop.sem_num =  0;            // Semaphore number
    sop.sem_op  =  1;            // Semaphore operation is Lock
    sop.sem_flg =  0;            // Operation flag
    int32_t err = semop(this->shared_memory_map_[key].sem_id, &sop, 1);
    if (err < 0) {
        printf("ERROR: unlock_memory() semop() error=%d key=%d\n", errno, key);
    }
    return;
}

void hako::utils::HakoSharedMemory::destroy_memory(int32_t key)
{
    void *addr = this->shared_memory_map_[key].addr;
    if (addr != nullptr) {
        (void)shmdt(addr);
        (void)shmctl (this->shared_memory_map_[key].shm_id, IPC_RMID, 0);
        (void)semctl(this->shared_memory_map_[key].sem_id, 1, IPC_RMID, NULL);
        this->shared_memory_map_.erase(key);
    }
    return;
}