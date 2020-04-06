namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using KafkaFlow.Producers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class ProducerConfigurationBuilder : IProducerConfigurationBuilder
    {
        private readonly Type producerType;

        private string topic;
        private ProducerConfig baseProducerConfig;
        private KafkaFlow.Acks? acks;

        private readonly List<Factory<IMessageMiddleware>> middlewaresFactories = new List<Factory<IMessageMiddleware>>();

        public ProducerConfigurationBuilder(IServiceCollection services, Type type)
        {
            this.ServiceCollection = services;
            this.producerType = type;
        }

        public IServiceCollection ServiceCollection { get; }

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

        public IProducerConfigurationBuilder UseMiddleware<T>()
            where T : class, IMessageMiddleware
        {
            return this.UseMiddleware(provider => provider.GetRequiredService<T>());
        }

        public IProducerConfigurationBuilder UseMiddleware<T>(Factory<T> factory)
            where T : class, IMessageMiddleware
        {
            this.ServiceCollection.TryAddSingleton<IMessageMiddleware, T>();
            this.ServiceCollection.TryAddSingleton<T>();
            this.middlewaresFactories.Add(factory);
            return this;
        }

        public ProducerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var configuration = new ProducerConfiguration(
                clusterConfiguration,
                this.topic,
                this.acks,
                this.middlewaresFactories,
                this.baseProducerConfig ?? new ProducerConfig());

            this.ServiceCollection.AddTransient(
                typeof(IMessageProducer<>).MakeGenericType(this.producerType),
                provider => Activator.CreateInstance(
                    typeof(MessageProducer<>).MakeGenericType(this.producerType),
                    provider,
                    configuration));

            return configuration;
        }
    }
}
