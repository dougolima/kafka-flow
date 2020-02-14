namespace KafkaFlow.Producers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration.Producers;
    using KafkaFlow.Extensions;

    public class MessageProducer<TProducer> : IMessageProducer<TProducer>
    {
        private readonly ProducerConfiguration configuration;
        private readonly IMessageSerializer serializer;
        private readonly IMessageCompressor compressor;
        private readonly IProducer<byte[], byte[]> producer;
        private readonly MiddlewareExecutor middlewareExecutor;

        public MessageProducer(
            IServiceProvider serviceProvider,
            ProducerConfiguration configuration)
        {
            this.configuration = configuration;

            this.serializer = (IMessageSerializer)serviceProvider.GetService(configuration.Serializer);
            this.compressor = (IMessageCompressor)serviceProvider.GetService(configuration.Compressor);

            this.producer = new ProducerBuilder<byte[], byte[]>(configuration.GetKafkaConfig()).Build();

            this.middlewareExecutor = new MiddlewareExecutor(this.configuration.Middlewares, serviceProvider);
        }

        public Task ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            Dictionary<string, byte[]> headers = null)
        {
            var serializedMessage = this.serializer.Serialize(message);
            var compressedMessage = this.compressor.Compress(serializedMessage);
            var messageKey = Encoding.UTF8.GetBytes(partitionKey);

            return this.middlewareExecutor.Execute(
                new MessageContext(
                    new ProducerMessage(messageKey, compressedMessage, headers),
                    this.configuration.Serializer,
                    this.configuration.Compressor,
                    topic)
                {
                    MessageType = message.GetType()
                },
                async context =>
                {
                    var result = await this.producer
                        .ProduceAsync(
                            context.Topic,
                            new Message<byte[], byte[]>
                            {
                                Key = context.Message.Key,
                                Value = context.Message.Value,
                                Headers = context.Message.Headers.ToKafkaHeaders(),
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
            Dictionary<string, byte[]> headers = null)
        {
            return this.ProduceAsync(
                this.configuration.Topic,
                partitionKey,
                message,
                headers);
        }
    }
}
