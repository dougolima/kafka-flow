namespace KafkaFlow.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IConsumerWorker : IWorker
    {
        ValueTask EnqueueAsync(MessageContext context);

        Task StartAsync(CancellationToken stopCancelltionToken = default);

        Task StopAsync();
    }
}
