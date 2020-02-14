namespace KafkaFlow
{
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    public interface IKafkaBus
    {
        KafkaConfiguration Configuration { get; }

        Task StartAsync();

        Task StopAsync();
    }
}
