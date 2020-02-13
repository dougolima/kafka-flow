namespace Kafka.Configuration.Producers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using Kafka.Extensions;
    using Kafka.Producers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class ProducerConfigurationBuilder<TProducer> : IProducerConfigurationBuilder
    {
        private readonly IServiceCollection services;

        private string topic;
        private Type serializer;
        private Type compressor;
        private ProducerConfig baseProducerConfig;
        private Acks? acks;

        private readonly List<MiddlewareDefinition> middlewares = new List<MiddlewareDefinition>();

        public ProducerConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public ProducerConfigurationBuilder<TProducer> Topic(string topic)
        {
            this.topic = topic;
            return this;
        }

        public ProducerConfigurationBuilder<TProducer> UseSerializer<TSerializer>() where TSerializer : IMessageSerializer
        {
            this.serializer = typeof(TSerializer);
            return this;
        }

        public ProducerConfigurationBuilder<TProducer> UseCompressor<TCompessor>() where TCompessor : IMessageCompressor
        {
            this.compressor = typeof(TCompessor);
            return this;
        }

        public ProducerConfigurationBuilder<TProducer> WithNoCompressor()
        {
            this.compressor = typeof(NullMessageCompressor);
            return this;
        }

        public ProducerConfigurationBuilder<TProducer> WithProducerConfig(ProducerConfig config)
        {
            this.baseProducerConfig = config;
            return this;
        }

        public ProducerConfigurationBuilder<TProducer> WithAcks(Acks acks)
        {
            this.acks = acks;
            return this;
        }

        public ProducerConfigurationBuilder<TProducer> UseMiddleware<TMiddleware>(Action<TMiddleware> configurator)
            where TMiddleware : IMessageMiddleware
        {
            this.middlewares.Add(new MiddlewareDefinition(
                typeof(TMiddleware),
                middleware => configurator((TMiddleware)middleware)));

            return this;
        }

        public ProducerConfigurationBuilder<TProducer> UseMiddleware<TMiddleware>()
            where TMiddleware : IMessageMiddleware
        {
            return this.UseMiddleware<TMiddleware>(configurator => { });
        }

        public ProducerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var combinedMiddlewares = clusterConfiguration.ProducersMiddlewares.Concat(this.middlewares);

            var configuration = new ProducerConfiguration(
                clusterConfiguration,
                this.topic,
                this.serializer,
                this.compressor,
                this.acks,
                combinedMiddlewares,
                this.baseProducerConfig ?? new ProducerConfig());

            this.services.TryAddSingleton(configuration.Serializer);
            this.services.TryAddSingleton(configuration.Compressor);

            this.services.AddTransient<IMessageProducer<TProducer>>(
                provider =>
                    new MessageProducer<TProducer>(
                        provider,
                        configuration));

            this.services.AddMiddlewares(configuration.Middlewares);

            return configuration;
        }
    }
}
