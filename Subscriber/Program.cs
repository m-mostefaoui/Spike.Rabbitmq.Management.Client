using System;

namespace Subscriber
{
    using System.IO;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using Microsoft.Extensions.Configuration;
    using RabbitMQ.Client;

    class Program
    {
        //private const string HostUrl = "rabbitmq://localhost/ParcelVision.Retail";
        //private const string User = "pvRetailDev";
        //private const string Password = "pvRetailDev";

        private static IRabbitMqHost host;

        public static void Main()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            string testChestnutQueueName = "spike-test-queue";
            string testPoplarQueueName = "spike-test-poplar-queue";

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                host = cfg.Host(new Uri(config["HostUrl"]), h =>
                {
                    h.Username(config["Username"]);
                    h.Password(config["Password"]);
                });
                
                cfg.ReceiveEndpoint(host, testChestnutQueueName, x =>
                {
                    x.Consumer<MessageConsumer>();
                });

                cfg.ReceiveEndpoint(host, testPoplarQueueName, x =>
                {
                    x.Consumer<MessageConsumer>();
                });
            });

            busControl.Start();

            Console.WriteLine("Listening for messages. Hit <return> to quit.");
            Console.ReadLine();

            busControl.Stop();
        }

    }
}
