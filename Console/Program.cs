namespace Console
{
    using System;
    using System.Threading.Tasks;
    using PowerArgs;
    using Spike.RabbitMq.Management.Client;

    class Program
    {
        private const string HostName = "localhost";
        private const string Username = "pvRetailDev";
        private const string Password = "pvRetailDev";

        private static async Task Main(string[] args)
        {
            do
            {                
                try
                {
                    var parsed = Args.Parse<CommandLineArgs>(args);

                    using (var managementClient = new RabbitMqManagementClient(HostName, Username, Password))
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