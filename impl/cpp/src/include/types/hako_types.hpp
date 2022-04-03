#ifndef _HAKO_TYPES_HPP_
#define _HAKO_TYPES_HPP_

#include <cstdint>
#include <memory>

/*
 * usec
 */
typedef int64_t HakoTimeType;

typedef struct {
    void (*callback_start) ();
    void (*callback_stop) ();
    void (*callback_reset) ();
} AssetCallbackType;

#endif /* _HAKO_TYPES_HPP_ */