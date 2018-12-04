namespace Spike.RabbitMq.Management.Client
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EasyNetQ.Management.Client;
    using EasyNetQ.Management.Client.Model;

    public class RabbitMqManagementClient : IRabbitMqManagementClient, IDisposable
    {
        public string HostUrl { get; }
        public string Username { get; }
        public string VirtualHost { get; }

        // EasyNetQ
        private readonly ManagementClient managementClient;

        public RabbitMqManagementClient(string hostUrl, string username, string password, string virtualHost = "ParcelVision.Retail")
        {
            if (string.IsNullOrEmpty(hostUrl)) throw new ArgumentException("hostUrl is null or empty");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("username is null or empty");
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("password is null or empty");

            HostUrl = hostUrl;
            Username = username;
            VirtualHost = virtualHost;

            managementClient = new ManagementClient(hostUrl, username, password);
        }

        public async Task CreateBinding(string queueName, string token)
        {
            var vhost = await managementClient.GetVhostAsync(VirtualHost);
            
            var exchanges = await managementClient.GetExchangesAsync();
            var filteredExchanges = exchanges.Where(x => x.Name.EndsWith(token, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (filteredExchanges.Any())
            {
                var createdQueue = await CreateQueue(queueName, vhost);
                foreach (var exchange in filteredExchanges)
                {
                    Console.WriteLine($"Exchange {exchange.Name}");
                    var createdExchange = await CreateExchange(exchange.Name, vhost);
                    await CreateBinding(createdExchange, createdQueue);
                }
            }
        }

        private async Task<Queue> CreateQueue(string queueName, Vhost vhost)
        {
            var (found, foundQueue) = await GetQueue(queueName, vhost);
            if (!found)
            {
                var createdQueue = await managementClient.CreateQueueAsync(new QueueInfo(queueName), vhost);
                return createdQueue;
            }

            return foundQueue;
        }

        private async Task<(bool found, Queue queue)> GetQueue(string queueName, Vhost vhost)
        {
            Queue queue = null;
            try
            {
                queue = await managementClient.GetQueueAsync(queueName, vhost);
            }
            catch (Exception)
            {
                // Log it
            }

            return (queue != null, queue);
        }

        private async Task<Exchange> CreateExchange(string exchangeName, Vhost vhost)
        {
            var (found, foundExchange) = await GetExchange(exchangeName, vhost);
            if (!found)
            {
                var createdExchange =
                    await managementClient.CreateExchangeAsync(
                        new ExchangeInfo(exchangeName, "fanout"), vhost);
                return createdExchange;
            }

            return foundExchange;
        }

        private async Task<(bool found, Exchange exchange)> GetExchange(string exchangeName, Vhost vhost)
        {
            Exchange exchange = null;

            try
            {
                exchange = await managementClient.GetExchangeAsync(exchangeName, vhost);
            }
            catch (UnexpectedHttpStatusCodeException)
            {
                // Log it
            }

            return (exchange != null, exchange);
        }

        private async Task CreateBinding(Exchange exchange, Queue queue, string routingKey = "")
        {
            await managementClient.CreateBinding(exchange, queue, new BindingInfo(routingKey));
        }

        public void Dispose()
        {
            managementClient?.Dispose();
        }
    }
}