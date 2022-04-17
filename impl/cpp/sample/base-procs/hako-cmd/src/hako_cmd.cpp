#include <hako.hpp>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <signal.h>


static void hako_cmd_signal_handler(int sig)
{
    hako::logger::get("cmd")->info("SIGNAL RECV: {0}", sig);
}

int main(int argc, const char* argv[])
{
    if (argc != 3) {
        printf("Usage: %s <delta_msec> <asset_name>\n", argv[0]);
        return 1;
    }
    signal(SIGINT, hako_cmd_signal_handler);
    signal(SIGTERM, hako_cmd_signal_handler);

    HakoTimeType delta_usec = strtol(argv[1], NULL, 10) * 1000;
    std::string asset_name = argv[2];

    hako::destroy();
    return 0;
}
