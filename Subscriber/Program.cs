using System;

namespace Subscriber
{
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using RabbitMQ.Client;

    class Program
    {
        private const string HostUrl = "rabbitmq://localhost/ParcelVision.Retail";
        private const string User = "pvRetailDev";
        private const string Password = "pvRetailDev";

        private static IRabbitMqHost host;
        public static void Main()
        {
            string testChestnutQueueName = "spike-test-queue";
            string testPoplarQueueName = "spike-test-poplar-queue";

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                host = cfg.Host(new Uri(HostUrl), h =>
                {
                    h.Username(User);
                    h.Password(Password);
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

        private static IBusControl ConfigureBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri(HostUrl), h =>
                {
                    h.Username(User);
                    h.Password(Password);
                });
            });
        }
    }
}
