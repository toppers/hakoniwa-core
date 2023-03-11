using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace Hakoniwa.PluggableAsset.Communication.Method.Mqtt
{
    public static class HakoMqtt
    {
        private static IMqttClient mqttClient = new MqttFactory().CreateMqttClient();
        private static string mqtt_ipaddr;
        private static int mqtt_port;
        private static List<string> topics = new List<string>();
        private static Dictionary<string, byte[]> buffers = new Dictionary<string, byte[]>();

        public static async Task Connect(string broker_ip, int broker_port)
        {
            mqtt_ipaddr = broker_ip;
            mqtt_port = broker_port;

            var mqttClientOptions = new MqttClientOptionsBuilder()
                //.WithTcpServer("mqtt://" + broker_ip + ":" + broker_port)
                .WithTcpServer(broker_ip, broker_port)
                .Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }
        private static async Task Recconect()
        {
            while (mqttClient.IsConnected == false)
            {
                await Connect(mqtt_ipaddr, mqtt_port);
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
            topics.Add(topic);
        }
        public static async Task StartSubscrive()
        {
            var mqttFactory = new MqttFactory();
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder();
            foreach (var topic in topics)
            {
                var next = mqttSubscribeOptions.WithTopicFilter(
                    f =>
                    {
                        f.WithTopic(topic);
                    }
                );
                mqttSubscribeOptions = next;
            }
            var args = mqttSubscribeOptions.Build();
            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine("Received application message.");
                string topic_name = e.ApplicationMessage.ResponseTopic;
                byte[] data = e.ApplicationMessage.Payload;
                buffers[topic_name] = data;
                return Task.CompletedTask;
            };
            while (true)
            {
                if (mqttClient.IsConnected == false)
                {
                    await Recconect();
                    continue;
                }
                await mqttClient.SubscribeAsync(args, CancellationToken.None);
            }
        }
    }
}
