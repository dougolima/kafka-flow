namespace KafkaFlow.Configuration.Consumers
{
    public interface IConsumerConfigurationBuilder
    {
        ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration);
    }
}
