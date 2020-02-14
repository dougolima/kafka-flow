namespace KafkaFlow.Configuration.Producers
{
    public interface IProducerConfigurationBuilder
    {
        ProducerConfiguration Build(ClusterConfiguration clusterConfiguration);
    }
}
