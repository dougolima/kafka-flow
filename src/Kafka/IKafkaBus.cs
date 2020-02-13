namespace Kafka
{
    using System.Threading.Tasks;
    using Kafka.Configuration;

    public interface IKafkaBus
    {
        KafkaConfiguration Configuration { get; }

        Task StartAsync();

        Task StopAsync();
    }
}
