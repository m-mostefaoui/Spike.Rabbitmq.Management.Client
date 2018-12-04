namespace Console
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using PowerArgs;
    using Spike.RabbitMq.Management.Client;

    class Program
    {
        private static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            do
            {                
                try
                {
                    var parsed = Args.Parse<CommandLineArgs>(args);

                    using (var managementClient = new RabbitMqManagementClient(config["HostUrl"], config["Username"], config["Password"]))
                    {
                        await managementClient.CreateBinding(parsed.QueueName, parsed.Token);

                        Console.WriteLine($"Binding queue {parsed.QueueName} with {parsed.Token} successfully.");
                    }
                }
                catch (ArgException ex)
                {
                    Console.WriteLine(ex.Message);
                }       
            } while (true);
        }
    }
}