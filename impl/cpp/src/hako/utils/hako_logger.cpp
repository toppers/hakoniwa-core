#include "utils/hako_logger.hpp"
#include "spdlog/sinks/rotating_file_sink.h"

static std::shared_ptr<spdlog::logger> hako_logger = nullptr;

void hako::utils::logger::init()
{
    hako_logger = spdlog::rotating_logger_mt("hako", 
        HAKO_LOGGER_FILEPATH, 
        HAKO_LOGGER_MAXSIZE, 
        HAKO_LOGGER_ROTNUM);
    hako_logger->info("hako logger initialized");
    return;
}

std::shared_ptr<spdlog::logger> hako::utils::logger::get()
{
    if (hako_logger == nullptr) {
        hako::utils::logger::init();
    }
    return hako_logger;
}