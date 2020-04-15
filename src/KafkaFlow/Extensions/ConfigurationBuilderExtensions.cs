namespace KafkaFlow
{
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    public static class ConfigurationBuilderExtensions
    {
        public static IProducerConfigurationBuilder WithProducerConfig(this IProducerConfigurationBuilder builder, ProducerConfig config)
        {
            ((ProducerConfigurationBuilder) builder).WithProducerConfig(config);
            return builder;
        }
    }
}
