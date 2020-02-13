namespace Kafka.Consumers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    public interface IWorkerPool
    {
        Task StartAsync(
            IConsumer<byte[], byte[]> consumer,
            IReadOnlyCollection<TopicPartition> partitions);

        Task StopAsync();

        Task EnqueueAsync(ConsumerMessage message);
    }
}