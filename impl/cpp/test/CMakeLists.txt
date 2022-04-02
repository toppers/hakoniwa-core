set(HAKO_CMAKE_VERSION ${HAKO_CMAKE_VERSION})

project(hakoniwa-core
    LANGUAGES C CXX
)

find_package(GTest REQUIRED)

include(GoogleTest)

add_executable(
    hakoniwa-core-test
    api/main.cpp
)

target_include_directories(
    hakoniwa-core-test
    PRIVATE ${GTEST_INCLUDE_DIRS}
)

target_link_libraries(hakoniwa-core-test 
    -pthread
    GTest::GTest
)

gtest_add_tests(TARGET hakoniwa-core-test)
