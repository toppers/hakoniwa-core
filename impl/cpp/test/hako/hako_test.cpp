#include <gtest/gtest.h>
#include "hako.hpp"

class HakoTest : public ::testing::Test {
protected:
    static void SetUpTestCase()
    {
    }
    static void TearDownTestCase()
    {
    }
    virtual void SetUp()
    {
    }
    virtual void TearDown()
    {
    }

};

TEST_F(HakoTest, IHakoMasterController_01)
{
    //prepare
    hako::init();

    HakoTimeType max_delay_usec = 100000ULL;
    HakoTimeType delta_usec = 10000ULL;

    //do
    std::shared_ptr<hako::IHakoMasterController> hako_master = hako::create_master();
    hako_master->set_config_simtime(max_delay_usec, delta_usec);

    EXPECT_EQ(hako_master->get_max_deltay_time_usec(), max_delay_usec);
    EXPECT_EQ(hako_master->get_delta_time_usec(), delta_usec);

    //done
    hako::destroy();
}

TEST_F(HakoTest, IHakoAssetController_01)
{
    //prepare
    hako::init();

    HakoTimeType max_delay_usec = 100000ULL;
    HakoTimeType delta_usec = 10000ULL;
    std::shared_ptr<hako::IHakoMasterController> hako_master = hako::create_master();
    hako_master->set_config_simtime(max_delay_usec, delta_usec);

    //do
    std::shared_ptr<hako::IHakoAssetController> hako_asset = hako::create_asset_controller();

    AssetCallbackType callback;
    callback.reset = nullptr;
    callback.start = nullptr;
    callback.stop = nullptr;
    auto ret = hako_asset->asset_register("TestAsset", callback);
    EXPECT_TRUE(ret);
    ret = hako_asset->asset_unregister("TestAsset");
    EXPECT_TRUE(ret);

    //done
    hako::destroy();
}

std::shared_ptr<hako::IHakoSimulationEventController> sim_ctrl = nullptr;

static void reset_callback()
{
    sim_ctrl->reset_feedback("TestAsset", true);
}
static void start_callback()
{
    sim_ctrl->start_feedback("TestAsset", true);
}
static void stop_callback()
{
    sim_ctrl->stop_feedback("TestAsset", true);
}
TEST_F(HakoTest, IHakoSimulationEventController_01)
{
    //prepare
    hako::init();

    HakoTimeType max_delay_usec = 100000ULL;
    HakoTimeType delta_usec = 10000ULL;
    std::shared_ptr<hako::IHakoMasterController> hako_master = hako::create_master();
    hako_master->set_config_simtime(max_delay_usec, delta_usec);

    std::shared_ptr<hako::IHakoAssetController> hako_asset = hako::create_asset_controller();
    sim_ctrl = hako::get_simevent_controller();

    AssetCallbackType callback;
    callback.reset = reset_callback;
    callback.start = start_callback;
    callback.stop = stop_callback;
    auto ret = hako_asset->asset_register("TestAsset", callback);
    EXPECT_TRUE(ret);

    sim_ctrl = hako::get_simevent_controller();

    //do
    ret = sim_ctrl->start();
    EXPECT_TRUE(ret);

    EXPECT_EQ(sim_ctrl->state(), HakoSim_Running);

    ret = sim_ctrl->stop();
    EXPECT_TRUE(ret);
    EXPECT_EQ(sim_ctrl->state(), HakoSim_Stopped);

    ret = sim_ctrl->reset();
    EXPECT_TRUE(ret);
    EXPECT_EQ(sim_ctrl->state(), HakoSim_Stopped);

    //done
    ret = hako_asset->asset_unregister("TestAsset");
    EXPECT_TRUE(ret);
    hako::destroy();
}

static void reset_callback_02()
{
    sim_ctrl->reset_feedback("TestAsset_02", true);
}
static void start_callback_02()
{
    sim_ctrl->start_feedback("TestAsset_02", true);
}
static void stop_callback_02()
{
    sim_ctrl->stop_feedback("TestAsset_02", true);
}

TEST_F(HakoTest, IHakoSimulationEventController_02)
{
    //prepare
    hako::init();

    HakoTimeType max_delay_usec = 100000ULL;
    HakoTimeType delta_usec = 10000ULL;
    std::shared_ptr<hako::IHakoMasterController> hako_master = hako::create_master();
    hako_master->set_config_simtime(max_delay_usec, delta_usec);

    std::shared_ptr<hako::IHakoAssetController> hako_asset = hako::create_asset_controller();
    sim_ctrl = hako::get_simevent_controller();

    AssetCallbackType callback;
    callback.reset = reset_callback;
    callback.start = start_callback;
    callback.stop = stop_callback;
    auto ret = hako_asset->asset_register("TestAsset", callback);
    EXPECT_TRUE(ret);

    AssetCallbackType callback_02;
    callback_02.reset = reset_callback_02;
    callback_02.start = start_callback_02;
    callback_02.stop = stop_callback_02;
    ret = hako_asset->asset_register("TestAsset_02", callback_02);
    EXPECT_TRUE(ret);

    sim_ctrl = hako::get_simevent_controller();

    //do
    ret = sim_ctrl->start();
    EXPECT_TRUE(ret);

    EXPECT_EQ(sim_ctrl->state(), HakoSim_Running);

    ret = sim_ctrl->stop();
    EXPECT_TRUE(ret);
    EXPECT_EQ(sim_ctrl->state(), HakoSim_Stopped);

    ret = sim_ctrl->reset();
    EXPECT_TRUE(ret);
    EXPECT_EQ(sim_ctrl->state(), HakoSim_Stopped);

    //done
    ret = hako_asset->asset_unregister("TestAsset");
    EXPECT_TRUE(ret);
    ret = hako_asset->asset_unregister("TestAsset_02");
    EXPECT_TRUE(ret);
    hako::destroy();
}