using System.Threading.Tasks;

namespace Spike.RabbitMq.Management.Client
{
    public interface IRabbitMqManagementClient
    {
        string HostUrl { get; }
        string Username { get; }
        string VirtualHost { get; }

        Task CreateBinding(string queueName, string token);
    }
}