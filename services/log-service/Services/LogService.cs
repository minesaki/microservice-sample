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
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
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
        }

        public static void Shutdown()
        {
            channel.Dispose();
            con.Dispose();
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
