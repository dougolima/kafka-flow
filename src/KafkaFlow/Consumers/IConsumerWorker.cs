namespace KafkaFlow.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal interface IConsumerWorker : IWorker
    {
        ValueTask EnqueueAsync(ConsumeResult<byte[], byte[]> message);

        Task StartAsync(CancellationToken stopCancelltionToken = default);

        Task StopAsync();
    }
}
