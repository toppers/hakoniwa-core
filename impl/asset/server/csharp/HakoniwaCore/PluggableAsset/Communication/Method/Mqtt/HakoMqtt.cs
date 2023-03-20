#if NO_USE_GRPC
#else
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hakoniwa.Core.Utils.Logger;
using MQTTnet;
using MQTTnet.Client;

namespace Hakoniwa.PluggableAsset.Communication.Method.Mqtt
{
    public static class HakoMqtt
    {
        private static IMqttClient mqttClient = new MqttFactory().CreateMqttClient();
        private static string mqtt_ipaddr = null;
        private static int mqtt_port;
        private static List<string> sub_topics = new List<string>();
        private static Dictionary<string, byte[]> buffers = new Dictionary<string, byte[]>();

        public static async Task Connect(string broker_ip, int broker_port)
        {

            if (mqtt_ipaddr == null)
            {
                mqtt_ipaddr = broker_ip;
                mqtt_port = broker_port;
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithClientId("hako-mqtt_unity_sender")
                    .WithTcpServer(broker_ip, broker_port)
                    .Build();

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            }
        }
        private static async Task Recconect()
        {
            while (mqttClient.IsConnected == false)
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithClientId("hako-mqtt_unity_sender")
                    .WithTcpServer(mqtt_ipaddr, mqtt_port)
                    .Build();

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                if (mqttClient.IsConnected)
                {
                    break;
                }
                await Task.Delay(1000);
            }
        }

        public static async Task SendMessage(string topic, byte[] data)
        {
            if (mqttClient.IsConnected == false)
            {
                await Recconect();
            }
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(data)
                .Build();
            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        }
        public static void AddSubscribeTopic(string topic)
        {
            SimpleLogger.Get().Log(Level.INFO, "AddSubscribeTopic=" + topic);
            buffers[topic] = null;
            sub_topics.Add(topic);
        }
        public static byte[] GetData(string topic)
        {
            return buffers[topic];
        }
        private static async Task SubRecconect(IMqttClient mqttSubClient)
        {
            while (mqttSubClient.IsConnected == false)
            {
                SimpleLogger.Get().Log(Level.INFO, "SubRecconect found connection failed... retry connect..");
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithClientId("hako-mqtt_unity_reciever")
                    .WithTcpServer(mqtt_ipaddr, mqtt_port)
                    .Build();

                await mqttSubClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                if (mqttSubClient.IsConnected)
                {
                    break;
                }
                await Task.Delay(1000);
            }
        }
        private static void OnAppMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            string topic_name = e.ApplicationMessage.ResponseTopic;
            SimpleLogger.Get().Log(Level.INFO, "recv topic topic_name=" + topic_name);
            byte[] data = e.ApplicationMessage.Payload;
            buffers[topic_name] = data;
        }
        public static void StartSubscribe()
        {
            var thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }
        private static async void ThreadMethod()
        {
            SimpleLogger.Get().Log(Level.INFO, "MQTT SUB THREAD ACTIVATED");
            SimpleLogger.Get().Log(Level.INFO, "sub_topics.Count=" + sub_topics.Count);
            if (sub_topics.Count == 0)
            {
                return;
            }
            SimpleLogger.Get().Log(Level.INFO, "START MQTT SUB SERVICE");
            var mqttFactory = new MqttFactory();
            IMqttClient mqttSubClient = mqttFactory.CreateMqttClient();
            mqttSubClient.ApplicationMessageReceivedAsync += e =>
            {
                OnAppMessage(e);
                return Task.CompletedTask;
            };
            await SubRecconect(mqttSubClient);
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(
                f =>
                {
                    f.WithTopic("hako_mqtt_0");
                }).Build();

            while (true)
            {
                if (mqttSubClient.IsConnected == false)
                {
                    await SubRecconect(mqttSubClient);
                    continue;
                }
                SimpleLogger.Get().Log(Level.INFO, "wait subscribe...");
                await mqttSubClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
            }

        }
    }
}
#endif