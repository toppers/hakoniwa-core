#include "utils/hako_logger.hpp"
#include "spdlog/sinks/rotating_file_sink.h"

void hako::utils::logger::init(const std::string &id)
{
    std::string logfile_path = HAKO_LOGGER_FILE_PREFIX + id + HAKO_LOGGER_FILE_EXTENSION;
    spdlog::rotating_logger_mt(id, 
        logfile_path, 
        HAKO_LOGGER_MAXSIZE, 
        HAKO_LOGGER_ROTNUM);
    spdlog::get(id)->info("hako logger[{0}] initialized", id);
    return;
}

std::shared_ptr<spdlog::logger> hako::utils::logger::get(const std::string &id)
{
    return spdlog::get(id);
}
