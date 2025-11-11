using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Ordering.Domain.Message;
using System.Data;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

namespace Ordering.Infraestructure.EventMessage
{
    public class RabbitMQOrderConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel;
        private string _queueName;
        private string _exchangeName;

        public RabbitMQOrderConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            _exchangeName = _configuration["TopicAndQueueNames:OrderCreatedTopic"];
        }
        private async Task ConfigureRabbitMQ()
        {

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Fanout);
            var queue = await _channel.QueueDeclareAsync();
            _queueName = queue.QueueName;
            await _channel.QueueBindAsync(_queueName, _exchangeName, string.Empty);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ConfigureRabbitMQ();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonConvert.DeserializeObject<OrderMessage>(json);
                await HandleMessage(message);
                await _channel.BasicAckAsync(args.DeliveryTag, false);
            };
            await _channel.BasicConsumeAsync(_queueName, false, consumer);
        }

        private Task HandleMessage(OrderMessage message)
        {
            //TODO: Logica real a servicio de dominio
            Console.WriteLine($" Order Received: {message.EmailAddress}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
            await base.StopAsync(cancellationToken);
        }

    }
}
