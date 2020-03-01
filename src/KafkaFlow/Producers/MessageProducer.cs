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
                new MessageContext(
                    message,
                    headers,
                    topic),
                async context =>
                {
                    var result = await this.producer
                        .ProduceAsync(
                            context.Topic,
                            new Message<byte[], byte[]>
                            {
                                Key = messageKey,
                                Value = (byte[])context.Message,
                                Headers = ((MessageHeaders)context.Headers).GetKafkaHeaders(),
                                Timestamp = Timestamp.Default
                            })
                        .ConfigureAwait(false);

                    context.Offset = result.Offset;
                    context.Partition = result.Partition;
                }
            );
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
    }
}
