namespace KafkaFlow
{
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    public interface IKafkaBus
    {
        KafkaConfiguration Configuration { get; }

        Task StartAsync(CancellationToken stopCancellationToken = default);

        Task StopAsync();
    }
}
