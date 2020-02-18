namespace KafkaFlow.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IConsumerWorker : IWorker
    {
        ValueTask EnqueueAsync(ConsumerMessage message);

        Task StartAsync(CancellationToken stopCancelltionToken = default);

        Task StopAsync();
    }
}
