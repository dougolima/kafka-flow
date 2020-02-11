namespace Kafka.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kafka.Configuration.Consumers;
    using Kafka.Configuration.Consumers.Raw;
    using Kafka.Configuration.Consumers.TypedHandler;
    using Kafka.Configuration.Producers;
    using Kafka.Consumers;
    using Kafka.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    public class ClusterConfigurationBuilder
    {
        private readonly IServiceCollection services;

        private readonly List<IProducerConfigurationBuilder> producers = new List<IProducerConfigurationBuilder>();
        private readonly List<IConsumerConfigurationBuilder> consumers = new List<IConsumerConfigurationBuilder>();

        private readonly List<MiddlewareDefinition> consumersMiddlewares = new List<MiddlewareDefinition>();
        private readonly List<MiddlewareDefinition> producersMiddlewares = new List<MiddlewareDefinition>();

        private IEnumerable<string> brokers;

        public ClusterConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public ClusterConfiguration Build(KafkaConfiguration kafkaConfiguration)
        {
            var configuration = new ClusterConfiguration(
                kafkaConfiguration,
                this.brokers.ToList(),
                this.consumersMiddlewares,
                this.producersMiddlewares);

            configuration.AddProducers(this.producers.Select(x => x.Build(configuration)));
            configuration.AddConsumers(this.consumers.Select(x => x.Build(configuration)));

            return configuration;
        }

        public ClusterConfigurationBuilder WithBrokers(IEnumerable<string> brokers)
        {
            this.brokers = brokers;
            return this;
        }

        public ClusterConfigurationBuilder UseConsumerMiddleware<TMiddleware>(Action<TMiddleware> configurator)
            where TMiddleware : IMessageMiddleware
        {
            this.consumersMiddlewares.Add(new MiddlewareDefinition(
                typeof(TMiddleware),
                middleware => configurator((TMiddleware)middleware)));

            return this;
        }

        public ClusterConfigurationBuilder UseConsumerMiddleware<TMiddleware>()
            where TMiddleware : IMessageMiddleware
        {
            return this.UseConsumerMiddleware<TMiddleware>(configurator => { });
        }

        public ClusterConfigurationBuilder UseProducerMiddleware<TMiddleware>(Action<TMiddleware> configurator)
            where TMiddleware : IMessageMiddleware
        {
            this.producersMiddlewares.Add(new MiddlewareDefinition(
                typeof(TMiddleware),
                middleware => configurator((TMiddleware)middleware)));

            return this;
        }

        public ClusterConfigurationBuilder UseProducerMiddleware<TMiddleware>()
            where TMiddleware : IMessageMiddleware
        {
            return this.UseConsumerMiddleware<TMiddleware>(configurator => { });
        }

        public ClusterConfigurationBuilder AddProducer<TProducer>(Action<ProducerConfigurationBuilder<TProducer>> producer)
        {
            var builder = new ProducerConfigurationBuilder<TProducer>(this.services);

            producer(builder);

            this.producers.Add(builder);

            return this;
        }

        public ClusterConfigurationBuilder AddRawConsumer<THandler>(Action<RawConsumerConfigurationBuilder> consumer) where THandler : IMessageHandler<byte[]>
        {
            var builder = new RawConsumerConfigurationBuilder(typeof(THandler), this.services);

            consumer(builder);

            this.consumers.Add(builder);

            return this;
        }

        public ClusterConfigurationBuilder AddTypedHandlerConsumer(Action<TypedHandlerConsumerConfigurationBuilder> consumer)
        {
            var builder = new TypedHandlerConsumerConfigurationBuilder(this.services);

            consumer(builder);

            this.consumers.Add(builder);

            return this;
        }

        //todo: add auth
    }
}
