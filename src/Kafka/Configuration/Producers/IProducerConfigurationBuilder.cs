namespace Kafka.Configuration.Producers
{
    public interface IProducerConfigurationBuilder
    {
        ProducerConfiguration Build(ClusterConfiguration clusterConfiguration);
    }
}
