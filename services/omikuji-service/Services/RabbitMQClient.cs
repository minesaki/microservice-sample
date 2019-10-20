using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

public class RabbitMQClient
{
    public void publish(string eventName)
    {
        // いったんpublishの度に生成しているが、パフォーマンス的に良くないかも
        ConnectionFactory factory = new RabbitMQ.Client.ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        using (IConnection con = factory.CreateConnection())
        using (IModel channel = con.CreateModel())
        {
            channel.ExchangeDeclare(eventName, "fanout", false, true);
            System.Console.WriteLine("### onActionExecuted ###");
            var msg = new { EventName = eventName, Time = DateTime.Now.ToString() };
            var json = JsonSerializer.Serialize(msg);
            try
            {
                channel.BasicPublish(eventName, "", null, Encoding.UTF8.GetBytes(json));
                System.Console.WriteLine("publish event succeeded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"publish event failed: {ex.Message}");
            }
        }
    }
}
