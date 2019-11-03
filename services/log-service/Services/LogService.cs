using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using System.Threading;

namespace LogService
{
    public class LogService : Log.LogBase
    {
        // RabbitMQのExchange名のうち、イベントログを生成するものを記載
        // ※イベントごとにExchangeを分けるべきなのか等、いろいろこれから調査する・・・
        private readonly static string[] exchanges = {
            "OmikujiDrawn"
        };

        private static IConnection con;
        private static IModel channel;
        private static EventingBasicConsumer consumer;

        public static void Init()
        {
            System.Console.WriteLine("Initializing...");

            string rabbitMQHostName = Environment.GetEnvironmentVariable("MQ_HostName");
            string rabbitMQUserName = Environment.GetEnvironmentVariable("MQ_UserName");
            string rabbitMQPassword = Environment.GetEnvironmentVariable("MQ_Password");

            if (string.IsNullOrWhiteSpace(rabbitMQHostName) ||
                string.IsNullOrWhiteSpace(rabbitMQUserName) ||
                string.IsNullOrWhiteSpace(rabbitMQPassword))
            {
                return;
            }

            int retryCount = 0;
            bool connectRabbitMQSucceeded = false;
            while (!connectRabbitMQSucceeded && retryCount < 10)
            {
                try
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = rabbitMQHostName,
                        UserName = rabbitMQUserName,
                        Password = rabbitMQPassword
                    };
                    con = factory.CreateConnection();
                    channel = con.CreateModel();

                    foreach (var exchange in exchanges)
                    {
                        // Exchange生成
                        channel.ExchangeDeclare(exchange, "fanout", false, true);
                        // Queue生成
                        var queueName = channel.QueueDeclare().QueueName;
                        // Bind Queue
                        channel.QueueBind(queueName, exchange, "");
                        // コンシューマー生成
                        LogService.consumer = new EventingBasicConsumer(channel);
                        // 受信イベント定義
                        consumer.Received += OnLogRequiredEventRaised;
                        // コンシューマー登録
                        channel.BasicConsume(queueName, true, consumer);
                    }

                    connectRabbitMQSucceeded = true;
                }
                catch
                {
                    retryCount++;
                    Thread.Sleep(1000);
                }
            }

            if (connectRabbitMQSucceeded)
            {
                System.Console.WriteLine("Initializing completed successfully.");
            }
            else
            {
                ShutdownRabbitMQ();
                System.Console.WriteLine("Initializing completed. (RabbitMQ init failed)");
            }
        }

        public static void ShutdownRabbitMQ()
        {
            if (channel != null)
            {
                channel.Dispose();
                channel = null;
            }
            if (con != null)
            {
                con.Dispose();
                con = null;
            }
        }

        private static void WriteEventLog(string time, string eventName)
        {
            Console.WriteLine($"[EventLog] Time: {time} / Name: {eventName}");
        }

        // RabbitMQからメッセージが飛んできた時の処理
        private static void OnLogRequiredEventRaised(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                Event msg = JsonSerializer.Deserialize<Event>(Encoding.UTF8.GetString(args.Body));
                WriteEventLog(msg.Time, msg.EventName);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        // gRPCで公開しているログ処理
        public override Task<LogReply> WriteLog(LogRequest request, ServerCallContext context)
        {
            try
            {
                WriteEventLog(DateTime.Now.ToString(), request.Message);

                return Task.FromResult(new LogReply
                {
                    Result = $"WriteLog succeeded: {request.Message}"
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new LogReply
                {
                    Result = $"WriteLog failed: {request.Message}, reason: {ex}"
                });
            }
        }

        class Event
        {
            public string EventName { get; set; }
            public string Time { get; set; }
        }
    }
}
