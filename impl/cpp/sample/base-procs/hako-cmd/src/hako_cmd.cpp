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
    std::vector<std::string> hako_status;

    hako_status.push_back("stopped");
    hako_status.push_back("runnable");
    hako_status.push_back("running");
    hako_status.push_back("stopping");
    hako_status.push_back("resetting");
    hako_status.push_back("error");
    hako_status.push_back("terminated");
    if (argc != 2) {
        printf("Usage: %s {start|stop|reset|status}\n", argv[0]);
        return 1;
    }
    signal(SIGINT, hako_cmd_signal_handler);
    signal(SIGTERM, hako_cmd_signal_handler);

    std::string cmd = argv[1];

    hako::logger::init("core");
    hako::logger::init("cmd");
    hako::logger::get("cmd")->info("cmd={0}", cmd);

    std::shared_ptr<hako::IHakoSimulationEventController> hako_sim_ctrl = hako::get_simevent_controller();
    if (cmd == "start") {
        printf("start\n");
        hako_sim_ctrl->start();
    }
    else if (cmd == "stop") {
        printf("stop\n");
        hako_sim_ctrl->stop();
    }
    else if (cmd == "reset") {
        printf("reset\n");
        hako_sim_ctrl->reset();
    }
    else if (cmd == "status") {
        printf("status=%s\n", hako_status[hako_sim_ctrl->state()].c_str());
    }
    else {
        printf("error\n");
    }
    hako::destroy();
    return 0;
}
