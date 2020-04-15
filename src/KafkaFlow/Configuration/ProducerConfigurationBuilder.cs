namespace KafkaFlow.Configuration
{
    using System;
    using Confluent.Kafka;
    using KafkaFlow.Producers;
    using Microsoft.Extensions.DependencyInjection;

    public class ProducerConfigurationBuilder : IProducerConfigurationBuilder
    {
        private readonly Type producerType;
        private readonly MiddlewareConfigurationBuilder middlewareConfigurationBuilder;

        private string topic;
        private ProducerConfig baseProducerConfig;
        private KafkaFlow.Acks? acks;

        public ProducerConfigurationBuilder(IServiceCollection services, Type type)
        {
            this.ServiceCollection = services;
            this.producerType = type;
            this.middlewareConfigurationBuilder = new MiddlewareConfigurationBuilder(services);
        }

        public IServiceCollection ServiceCollection { get; }

        public IProducerConfigurationBuilder AddMiddlewares(Action<IProducerMiddlewareConfigurationBuilder> middlewares)
        {
            middlewares(this.middlewareConfigurationBuilder);
            return this;
        }

        public IProducerConfigurationBuilder DefaultTopic(string topic)
        {
            this.topic = topic;
            return this;
        }

        public IProducerConfigurationBuilder WithProducerConfig(ProducerConfig config)
        {
            this.baseProducerConfig = config;
            return this;
        }

        public IProducerConfigurationBuilder WithAcks(KafkaFlow.Acks acks)
        {
            this.acks = acks;
            return this;
        }

        public ProducerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var configuration = new ProducerConfiguration(
                clusterConfiguration,
                this.topic,
                this.acks,
                this.middlewareConfigurationBuilder.Build(),
                this.baseProducerConfig ?? new ProducerConfig());

            this.ServiceCollection.AddSingleton(
                typeof(IMessageProducer<>).MakeGenericType(this.producerType),
                provider => Activator.CreateInstance(
                    typeof(MessageProducer<>).MakeGenericType(this.producerType),
                    provider,
                    configuration));

            return configuration;
        }
    }
}
