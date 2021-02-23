#ifndef _HAKONIWA_PROCESS_H_
#define _HAKONIWA_PROCESS_H_

#include <string>
#include <memory>
#include <thread>
#include <vector>

#include <sys/types.h>
#include <sys/stat.h>
#include <unistd.h>

namespace hakoniwa {

class ProcessManager {
public:
    ProcessManager() {}
    ~ProcessManager() {
    }
    bool invoke() {
        if (this->current_dir_ == nullptr) {
            return false;
        }
        if (this->binary_path_ == nullptr) {
            return false;
        }
        int ret = fork();
        printf("fork:%d\n", ret);
        if (ret < 0) {
            return false;
        }
        else if (ret == 0) {
            //another process context start
            ret = ProcessManager::monitoring(this);
            exit(ret);
        }
        this->exec_args[this->exec_arg_count] = NULL;
        this->status_is_running = true;
        this->pid_ = ret;
        return true;
    }
    bool is_running()
    {
        return this->status_is_running;
    }

    bool set_current_dir(std::string dir) {
        struct stat buf;
        int ret = stat(dir.c_str(), &buf);
        if (ret < 0) {
            return false;
        }
        this->current_dir_.reset();
        this->current_dir_ = std::make_unique<std::string>(dir);
        return true;
    }
    bool set_binary_path(std::string path) {
        struct stat buf;
        int ret = stat(path.c_str(), &buf);
        if (ret < 0) {
            return false;
        }
        this->binary_path_.reset();
        this->binary_path_ = std::make_unique<std::string>(path);
        this->the_args.clear();
        this->exec_arg_count = 0;
        this->add_option(path);
        return true;
    }
    bool add_option(std::string option) {
        if (this->binary_path_) {
            the_args.push_back(option);
            printf("add_option:%s\n", option.c_str());
            return true;
        }
        else {
            return false;
        }
    }
    int get_process_exeuted_result() {
        return this->process_executed_result;
    }

private:
    std::unique_ptr<std::string> binary_path_ = nullptr;
    std::unique_ptr<std::string> current_dir_ = nullptr;
    bool status_is_running = false;
    int process_executed_result = 0;
    int exec_arg_count = 0;
    char *exec_args[1024];
    std::vector<std::string> the_args;
    int pid_;
    void finish()
    {
        this->status_is_running = false;
    }
    static int monitoring(ProcessManager *mgrp)
    {
        printf("monitring:\n");
        int ret = chdir(mgrp->current_dir_->c_str());
        if (ret >= 0) {
            printf("exec_args_count=%ld\n", mgrp->the_args.size());
            int i;
            for (i = 0; i < mgrp->the_args.size(); i++) {
                printf("the_args[%d]=%s\n", i, mgrp->the_args.at(i).c_str());
                mgrp->exec_args[mgrp->exec_arg_count++] = (char*)mgrp->the_args.at(i).c_str();
            }
            mgrp->exec_args[mgrp->exec_arg_count] = NULL;
            ret = execv(mgrp->binary_path_->c_str(), mgrp->exec_args);
        }
        return ret;
    }
};

}

#endif /* _HAKONIWA_PROCESS_H_ */