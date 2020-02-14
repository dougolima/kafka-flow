namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    public interface IConsumerWorkerPool
    {
        Task StartAsync(
            IConsumer<byte[], byte[]> consumer,
            IReadOnlyCollection<TopicPartition> partitions);

        Task StopAsync();

        ValueTask EnqueueAsync(ConsumerMessage message);
    }
}
