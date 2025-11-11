using Microsoft.Extensions.Configuration;
using Ordering.Aplication.Messaging;
using RabbitMQ.Client;
using System.Text;

namespace Ordering.Infrastructure.EventMessage
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _exchangeName;

        public RabbitMQEventBus(IConnection connection, IChannel channel, IConfiguration configuration)
        {
            _connection = connection;
            _channel = channel;
            _exchangeName = configuration["TopicAndQueueNames:OrderCreatedTopic"];
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: string.Empty,
                mandatory: false,
                body: body
            );
        }
    }
}

