namespace KafkaFlow.Configuration.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using KafkaFlow.Consumers.DistribuitionStrategies;
    using KafkaFlow.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public abstract class ConsumerConfigurationBuilder<TBuilder>
        : IConsumerConfigurationBuilder
        where TBuilder : ConsumerConfigurationBuilder<TBuilder>
    {
        private readonly IServiceCollection services;

        private string topic;
        private string groupId;
        private AutoOffsetReset? autoOffsetReset;
        private int? autoCommitIntervalMs;
        private int? maxPollIntervalMs;
        private int workersCount;
        private int bufferSize;
        private bool? autoStoreOffsets;

        private ConfigurableDefinition<IDistribuitionStrategy> distribuitionStrategyDefinition =
            new ConfigurableDefinition<IDistribuitionStrategy>(
                typeof(BytesSumDistribuitionStrategy),
                (strategy, provider) => { });

        private readonly List<ConfigurableDefinition<IMessageMiddleware>> middlewares = new List<ConfigurableDefinition<IMessageMiddleware>>();

        protected ConsumerConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// Set the topic that will be used to read the messages
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns></returns>
        public TBuilder Topic(string topic)
        {
            this.topic = topic;
            return (TBuilder)this;
        }

        /// <summary>
        /// Set the group id used by the consumer
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public TBuilder WithGroupId(string groupId)
        {
            this.groupId = groupId;
            return (TBuilder)this;
        }

        /// <summary>
        /// Set the initial offset strategy used by new consumer groups.
        /// If your consumer group (set by method <see cref="WithGroupId"/>) has no offset stored in Kafka, this configuration will be used
        /// Use Earliest to read the topic from the beginning
        /// Use Latest to read only new messages in the topic
        /// </summary>
        /// <param name="autoOffsetReset"></param>
        /// <returns></returns>
        public TBuilder WithAutoOffsetReset(AutoOffsetReset autoOffsetReset)
        {
            this.autoOffsetReset = autoOffsetReset;
            return (TBuilder)this;
        }

        /// <summary>
        /// Set the interval used by the framework to commit the stored offsets in Kafka
        /// </summary>
        /// <param name="autoCommitIntervalMs">The interval in milliseconds</param>
        /// <returns></returns>
        public TBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs)
        {
            this.autoCommitIntervalMs = autoCommitIntervalMs;
            return (TBuilder)this;
        }

        /// <summary>
        /// Set the max interval between message consumption, if this time exceeds the consumer is considered failed and Kafka will revoke the assigned partitions
        /// </summary>
        /// <param name="maxPollIntervalMs">The interval in milliseconds</param>
        /// <returns></returns>
        public TBuilder WithMaxPollIntervalMs(int maxPollIntervalMs)
        {
            this.maxPollIntervalMs = maxPollIntervalMs;
            return (TBuilder)this;
        }

        /// <summary>
        /// Set the number of threads that will be used to consume the messages
        /// </summary>
        /// <param name="workersCount"></param>
        /// <returns></returns>
        public TBuilder WithWorkersCount(int workersCount)
        {
            this.workersCount = workersCount;
            return (TBuilder)this;
        }

        /// <summary>
        /// Set how many messages will be buffered for each worker
        /// </summary>
        /// <param name="size">The buffer size</param>
        /// <returns></returns>
        public TBuilder WithBufferSize(int size)
        {
            this.bufferSize = size;
            return (TBuilder)this;
        }

        /// <summary>
        /// Set the strategy to choose a worker when a message arrives
        /// </summary>
        /// <typeparam name="TStrategy">A class that implements the <see cref="IDistribuitionStrategy"/> interface</typeparam>
        /// <param name="configure">A handler to configure the strategy settings</param>
        /// <returns></returns>
        public TBuilder WithWorkDistribuitionStretagy<TStrategy>(Action<TStrategy, IServiceProvider> configure)
            where TStrategy : IDistribuitionStrategy
        {
            this.distribuitionStrategyDefinition =
                new ConfigurableDefinition<IDistribuitionStrategy>(
                    typeof(TStrategy),
                    (strategy, provider) => configure((TStrategy)strategy, provider));

            return (TBuilder)this;
        }

        /// <summary>
        /// Set the strategy to choose a worker when a message arrives
        /// </summary>
        /// <typeparam name="TStrategy">A class that implements the <see cref="IDistribuitionStrategy"/> interface</typeparam>
        /// <returns></returns>
        public TBuilder WithWorkDistribuitionStretagy<TStrategy>()
            where TStrategy : IDistribuitionStrategy
        {
            return this.WithWorkDistribuitionStretagy<TStrategy>((strategy, provider) => { });
        }

        /// <summary>
        /// Offsets will be stored after the execution of the handler and middlewares automatically, this is the default behaviour
        /// </summary>
        /// <returns></returns>
        public TBuilder WithAutoStoreOffsets()
        {
            this.autoStoreOffsets = true;
            return (TBuilder)this;
        }

        /// <summary>
        /// The Handler or Middleware should call the <see cref="MessageContext.StoreOffset()"/> manually
        /// </summary>
        /// <returns></returns>
        public TBuilder WithManualStoreOffsets()
        {
            this.autoStoreOffsets = false;
            return (TBuilder)this;
        }

        /// <summary>
        /// Register a middleware to be used when consuming messages
        /// </summary>
        /// <param name="configurator">A handler to configure the middleware</param>
        /// <typeparam name="TMiddleware">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        public TBuilder UseMiddleware<TMiddleware>(Action<TMiddleware, IServiceProvider> configurator)
            where TMiddleware : IMessageMiddleware
        {
            this.middlewares.Add(
                new ConfigurableDefinition<IMessageMiddleware>(
                    typeof(TMiddleware),
                    (middleware, provider) => configurator((TMiddleware)middleware, provider)));

            return (TBuilder)this;
        }

        /// <summary>
        /// Register a middleware to be used when consuming messages
        /// </summary>
        /// <typeparam name="TMiddleware">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        public TBuilder UseMiddleware<TMiddleware>()
            where TMiddleware : IMessageMiddleware
        {
            return this.UseMiddleware<TMiddleware>((middleware, provider) => { });
        }

        public virtual ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var combinedMiddlewares = clusterConfiguration.ConsumersMiddlewares.Concat(this.middlewares);

            var configuration = new ConsumerConfiguration(
                clusterConfiguration,
                this.topic,
                this.groupId,
                this.workersCount,
                this.bufferSize,
                this.distribuitionStrategyDefinition,
                combinedMiddlewares)
            {
                AutoOffsetReset = this.autoOffsetReset,
                AutoCommitIntervalMs = this.autoCommitIntervalMs,
                MaxPollIntervalMs = this.maxPollIntervalMs
            };

            configuration.AutoStoreOffsets = this.autoStoreOffsets ?? configuration.AutoStoreOffsets;

            this.services.TryAddTransient(configuration.DistribuitionStrategy.Type);
            this.services.AddMiddlewares(configuration.Middlewares);

            return configuration;
        }
    }
}
