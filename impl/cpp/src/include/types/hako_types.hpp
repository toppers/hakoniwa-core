#ifndef _HAKO_TYPES_HPP_
#define _HAKO_TYPES_HPP_

#include <cstdint>
#include <memory>
#include <sys/types.h>
#include <sys/ipc.h>

/*
 * usec
 */
typedef uint64_t HakoTimeType;

typedef struct {
    void (*callback_start) ();
    void (*callback_stop) ();
    void (*callback_reset) ();
} AssetCallbackType;


#define HAKO_SHARED_MEMORY_BASE_ID   0x00FF
#define HAKO_SHARED_MEMORY_ID_0     (0x0 + HAKO_SHARED_MEMORY_BASE_ID)

#endif /* _HAKO_TYPES_HPP_ */