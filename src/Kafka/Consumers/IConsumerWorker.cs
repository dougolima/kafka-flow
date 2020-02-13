namespace Kafka.Consumers
{
    using System.Threading.Tasks;

    public interface IConsumerWorker
    {
        ValueTask EnqueueAsync(ConsumerMessage message);

        Task StartAsync();

        Task StopAsync();
    }
}
