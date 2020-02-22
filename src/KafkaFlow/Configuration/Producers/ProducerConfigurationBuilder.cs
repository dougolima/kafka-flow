namespace KafkaFlow.Configuration.Producers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using KafkaFlow.Extensions;
    using KafkaFlow.Producers;
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

        /// <summary>
        /// Set the default topic to be used when producing messages
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns></returns>
        public ProducerConfigurationBuilder<TProducer> DefaultTopic(string topic)
        {
            this.topic = topic;
            return this;
        }

        /// <summary>
        /// Set the default serializer to be used when producing messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements the <see cref="IMessageSerializer"/> interface</typeparam>
        /// <returns></returns>
        public ProducerConfigurationBuilder<TProducer> UseSerializer<TSerializer>() where TSerializer : IMessageSerializer
        {
            this.serializer = typeof(TSerializer);
            return this;
        }

        /// <summary>
        /// Set the default compressor to be used when producing messages
        /// </summary>
        /// <typeparam name="TCompessor">A class that implements the <see cref="IMessageCompressor"/> interface</typeparam>
        /// <returns></returns>
        public ProducerConfigurationBuilder<TProducer> UseCompressor<TCompessor>() where TCompessor : IMessageCompressor
        {
            this.compressor = typeof(TCompessor);
            return this;
        }

        /// <summary>
        /// Configure the producer to use no compression
        /// </summary>
        /// <returns></returns>
        public ProducerConfigurationBuilder<TProducer> WithNoCompressor()
        {
            this.compressor = typeof(NullMessageCompressor);
            return this;
        }

        /// <summary>
        /// Set the Confluent <see cref="ProducerConfig"/> directly 
        /// </summary>
        /// <param name="config">The Confluent configuration</param>
        /// <returns></returns>
        public ProducerConfigurationBuilder<TProducer> WithProducerConfig(ProducerConfig config)
        {
            this.baseProducerConfig = config;
            return this;
        }

        /// <summary>
        /// Set the <see cref="Acks"/> to be used when producing messages
        /// </summary>
        /// <param name="acks"></param>
        /// <returns></returns>
        public ProducerConfigurationBuilder<TProducer> WithAcks(Acks acks)
        {
            this.acks = acks;
            return this;
        }

        /// <summary>
        /// Register a middleware to be used when producing messages
        /// </summary>
        /// <param name="configurator">A handler to configure the middleware</param>
        /// <typeparam name="TMiddleware">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        public ProducerConfigurationBuilder<TProducer> UseMiddleware<TMiddleware>(Action<TMiddleware, IServiceProvider> configurator)
            where TMiddleware : IMessageMiddleware
        {
            this.middlewares.Add(new MiddlewareDefinition(
                typeof(TMiddleware),
                (middleware, provider) => configurator((TMiddleware)middleware, provider)));

            return this;
        }

        /// <summary>
        /// Register a middleware to be used when producing messages
        /// </summary>
        /// <typeparam name="TMiddleware">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        public ProducerConfigurationBuilder<TProducer> UseMiddleware<TMiddleware>()
            where TMiddleware : IMessageMiddleware
        {
            return this.UseMiddleware<TMiddleware>((middleware, provider) => { });
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
