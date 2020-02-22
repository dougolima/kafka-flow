namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Configuration.Consumers;
    using KafkaFlow.Configuration.Consumers.Raw;
    using KafkaFlow.Configuration.Consumers.TypedHandler;
    using KafkaFlow.Configuration.Producers;
    using KafkaFlow.Consumers;
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

        /// <summary>
        /// Set the Kafka Brokers to be used
        /// </summary>
        /// <param name="brokers"></param>
        /// <returns></returns>
        public ClusterConfigurationBuilder WithBrokers(IEnumerable<string> brokers)
        {
            this.brokers = brokers;
            return this;
        }

        /// <summary>
        /// Register a middleware for consumers of the entire cluster
        /// </summary>
        /// <param name="configurator">A handler to configure the middleware</param>
        /// <typeparam name="TMiddleware">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        public ClusterConfigurationBuilder UseConsumerMiddleware<TMiddleware>(Action<IMessageMiddleware, IServiceProvider> configurator)
            where TMiddleware : IMessageMiddleware
        {
            this.consumersMiddlewares.Add(
                new MiddlewareDefinition(
                    typeof(TMiddleware),
                    (middleware, provider) => configurator((TMiddleware)middleware, provider)));

            return this;
        }

        /// <summary>
        /// Register a middleware for consumers of the entire cluster
        /// </summary>
        /// <typeparam name="TMiddleware">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        public ClusterConfigurationBuilder UseConsumerMiddleware<TMiddleware>()
            where TMiddleware : IMessageMiddleware
        {
            return this.UseConsumerMiddleware<TMiddleware>((middleware, provider) => { });
        }

        /// <summary>
        /// Register a middleware for producers of the entire cluster
        /// </summary>
        /// <param name="configurator">A handler to configure the middleware</param>
        /// <typeparam name="TMiddleware">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        public ClusterConfigurationBuilder UseProducerMiddleware<TMiddleware>(Action<TMiddleware, IServiceProvider> configurator)
            where TMiddleware : IMessageMiddleware
        {
            this.producersMiddlewares.Add(
                new MiddlewareDefinition(
                    typeof(TMiddleware),
                    (middleware, provider) => configurator((TMiddleware)middleware, provider)));

            return this;
        }

        /// <summary>
        /// Register a middleware for producers of the entire cluster
        /// </summary>
        /// <typeparam name="TMiddleware">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        public ClusterConfigurationBuilder UseProducerMiddleware<TMiddleware>()
            where TMiddleware : IMessageMiddleware
        {
            return this.UseConsumerMiddleware<TMiddleware>((middleware, provider) => { });
        }

        /// <summary>
        /// Adds a producer to the cluster
        /// </summary>
        /// <param name="producer">A handler to configure the producer</param>
        /// <typeparam name="TProducer">The class responsible for the production</typeparam>
        /// <returns></returns>
        public ClusterConfigurationBuilder AddProducer<TProducer>(Action<ProducerConfigurationBuilder<TProducer>> producer)
        {
            var builder = new ProducerConfigurationBuilder<TProducer>(this.services);

            producer(builder);

            this.producers.Add(builder);

            return this;
        }

        /// <summary>
        /// Adds a <see cref="RawConsumer"/> to the cluster
        /// </summary>
        /// <param name="consumer">A handler to configure the consumer</param>
        /// <typeparam name="THandler">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public ClusterConfigurationBuilder AddRawConsumer<THandler>(Action<RawConsumerConfigurationBuilder> consumer) where THandler : IMessageHandler<byte[]>
        {
            var builder = new RawConsumerConfigurationBuilder(typeof(THandler), this.services);

            consumer(builder);

            this.consumers.Add(builder);

            return this;
        }

        /// <summary>
        /// Adds a <see cref="TypedHandlerConsumer"/> to the cluster
        /// </summary>
        /// <param name="consumer">A handler to configure the consumer</param>
        /// <returns></returns>
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
