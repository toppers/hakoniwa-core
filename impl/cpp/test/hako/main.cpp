#include <stdio.h>
#include <gtest/gtest.h>

#include "hako.hpp"

int main(int argc, char *argv[])
{
    ::testing::InitGoogleTest(&argc, argv);

    hako::init();
    int result = RUN_ALL_TESTS();
    return result;
}
