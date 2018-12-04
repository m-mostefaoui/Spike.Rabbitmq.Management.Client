namespace Subscriber
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Newtonsoft.Json.Linq;

    public class MessageConsumer : IConsumer<JToken>
    {
        public Task Consume(ConsumeContext<JToken> context)
        {
            var jToken = context.Message;

            var fieldsCollector = new JsonFieldsCollector(jToken);
            var fields = fieldsCollector.GetAllFields();

            foreach (var field in fields)
            {
                Console.Out.WriteLineAsync($"{field.Key}: '{field.Value}'");
            }

            return Task.CompletedTask;
        }
    }
}