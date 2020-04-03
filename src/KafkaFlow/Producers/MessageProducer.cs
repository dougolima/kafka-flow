namespace KafkaFlow.Producers
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    public class MessageProducer<TProducer> : IMessageProducer<TProducer>
    {
        private readonly ProducerConfiguration configuration;
        private readonly IProducer<byte[], byte[]> producer;
        private readonly MiddlewareExecutor middlewareExecutor;

        public MessageProducer(
            IServiceProvider serviceProvider,
            ProducerConfiguration configuration)
        {
            this.configuration = configuration;

            this.producer = new ProducerBuilder<byte[], byte[]>(configuration.GetKafkaConfig()).Build();

            this.middlewareExecutor = new MiddlewareExecutor(this.configuration.MiddlewaresFactories, serviceProvider);
        }

        public Task ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            var messageKey = Encoding.UTF8.GetBytes(partitionKey);

            return this.middlewareExecutor.Execute(
                new MessageContext(message, messageKey, headers, topic),
                this.InternalProduce);
        }

        public Task ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            return this.ProduceAsync(
                this.configuration.Topic,
                partitionKey,
                message,
                headers);
        }

        private async Task InternalProduce(MessageContext context)
        {
            if (!(context.Message is byte[] value))
            {
                throw new InvalidOperationException(
                    $"{nameof(context.Message)} must be a byte array to be produced, it is a {context.Message.GetType().FullName}." +
                    "You should serialize or encode your message object using a middleware");
            }

            var result = await this.producer
                .ProduceAsync(
                    context.Topic,
                    new Message<byte[], byte[]>
                    {
                        Key = context.PartitionKey,
                        Value = value,
                        Headers = ((MessageHeaders)context.Headers).GetKafkaHeaders(),
                        Timestamp = Timestamp.Default
                    })
                .ConfigureAwait(false);

            context.Offset = result.Offset;
            context.Partition = result.Partition;
        }
    }
}
