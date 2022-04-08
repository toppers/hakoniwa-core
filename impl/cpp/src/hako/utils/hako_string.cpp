#include "hako_string.hpp"
#include <string.h>

std::shared_ptr<std::string> hako::utils::hako_fixed2string(const HakoFixedStringType &str)
{
    return std::make_shared<std::string>(str.data, str.len);
}

void hako::utils::hako_string2fixed(const std::string &src, HakoFixedStringType &dst)
{
    if (src.length() > HAKO_FIXED_STRLEN_MAX) {
        throw new std::bad_alloc();
    }
    memcpy(&dst.data[0], src.c_str(), src.length());
    dst.data[src.length()] = 0x0;
    return;
}
