namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    public interface IConsumerWorker : IWorker
    {
        ValueTask EnqueueAsync(ConsumerMessage message);

        Task StartAsync();

        Task StopAsync();
    }
}
