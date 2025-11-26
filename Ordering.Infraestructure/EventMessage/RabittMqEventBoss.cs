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
        private readonly string _exchangeType; // Para usar el tipo de exchange de la configuración
        private readonly string _queueName;    // Nombre de la cola donde quieres recibir el mensaje

        // Nombres de configuración
        private const string ExchangeKey = "TopicAndQueueNames:OrderCreatedTopic";
        private const string ExchangeTypeKey = "RabbitMQ:ExchangeType";
        private const string QueueKey = "TopicAndQueueNames:OrderQueueName"; // Asumimos que agregarás este nombre

        public RabbitMQEventBus(IConnection connection, IChannel channel, IConfiguration configuration)
        {
            _connection = connection;
            _channel = channel;

            // 1. Obtener nombres y tipos de la configuración
            _exchangeName = configuration[ExchangeKey] ?? "order_exchange";
            _exchangeType = configuration[ExchangeTypeKey] ?? ExchangeType.Fanout;

            // **IMPORTANTE**: Necesitas definir un nombre para tu cola. 
            // Podrías usar el nombre del exchange y agregarle "_queue", o leerlo de la configuración.
            _queueName = configuration[QueueKey] ?? $"{_exchangeName}_queue";

            // 2. Declarar el Exchange, la Queue y ligarlos.
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            // Declarar el Exchange
            _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: _exchangeType, // Usar el tipo de la configuración (ej. "fanout")
                durable: true        // Hacer el exchange duradero
            );

            // Declarar la Queue
            // 'durable: true' para que sobreviva a un reinicio de RabbitMQ
            // 'exclusive: false' para que pueda ser compartida por varios consumidores
            // 'autoDelete: false' para que no se borre al desconectarse el último consumidor
            _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Ligar la Queue al Exchange
            // Para 'fanout', el 'routingKey' se ignora (se puede usar string.Empty)
            _channel.QueueBindAsync(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: string.Empty // Clave de enrutamiento
            );
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            // NOTA: Para un exchange de tipo 'fanout', la 'routingKey' se ignora.
            // Para otros tipos como 'direct' o 'topic', deberías usar una routingKey relevante aquí.
            await _channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: string.Empty, // Se ignora en 'fanout'
                mandatory: false,
                body: body
            );
        }
    }
}