namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Consumers.DistributionStrategies;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class ConsumerConfigurationBuilder
        : IConsumerConfigurationBuilder
    {
        private readonly List<string> topics = new List<string>();
        private readonly ConsumerMiddlewareConfigurationBuilder middlewareConfigurationBuilder;

        private string groupId;
        private AutoOffsetReset? autoOffsetReset;
        private int? autoCommitIntervalMs;
        private int? maxPollIntervalMs;
        private int workersCount;
        private int bufferSize;
        private bool autoStoreOffsets = true;

        private Factory<IDistributionStrategy> distributionStrategyFactory = provider => new BytesSumDistributionStrategy();

        public ConsumerConfigurationBuilder(IServiceCollection services)
        {
            this.middlewareConfigurationBuilder = new ConsumerMiddlewareConfigurationBuilder(services);
            this.ServiceCollection = services;
        }

        public IServiceCollection ServiceCollection { get; }

        public IConsumerConfigurationBuilder Topic(string topic)
        {
            this.topics.Add(topic);
            return this;
        }

        public IConsumerConfigurationBuilder Topics(IEnumerable<string> topics)
        {
            this.topics.AddRange(topics);
            return this;
        }

        public IConsumerConfigurationBuilder Topics(params string[] topics) => this.Topics(topics.AsEnumerable());

        public IConsumerConfigurationBuilder WithGroupId(string groupId)
        {
            this.groupId = groupId;
            return this;
        }

        public IConsumerConfigurationBuilder WithAutoOffsetReset(AutoOffsetReset autoOffsetReset)
        {
            this.autoOffsetReset = autoOffsetReset;
            return this;
        }

        public IConsumerConfigurationBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs)
        {
            this.autoCommitIntervalMs = autoCommitIntervalMs;
            return this;
        }

        public IConsumerConfigurationBuilder WithMaxPollIntervalMs(int maxPollIntervalMs)
        {
            this.maxPollIntervalMs = maxPollIntervalMs;
            return this;
        }

        public IConsumerConfigurationBuilder WithWorkersCount(int workersCount)
        {
            this.workersCount = workersCount;
            return this;
        }

        public IConsumerConfigurationBuilder WithBufferSize(int size)
        {
            this.bufferSize = size;
            return this;
        }

        public IConsumerConfigurationBuilder WithWorkDistributionStrategy<T>(Factory<T> factory)
            where T : class, IDistributionStrategy
        {
            this.distributionStrategyFactory = factory;
            return this;
        }

        public IConsumerConfigurationBuilder WithWorkDistributionStrategy<T>()
            where T : class, IDistributionStrategy
        {
            this.ServiceCollection.TryAddTransient(typeof(T));
            this.distributionStrategyFactory = provider => provider.GetRequiredService<T>();
            return this;
        }

        public IConsumerConfigurationBuilder WithAutoStoreOffsets()
        {
            this.autoStoreOffsets = true;
            return this;
        }

        public IConsumerConfigurationBuilder WithManualStoreOffsets()
        {
            this.autoStoreOffsets = false;
            return this;
        }

        public IConsumerConfigurationBuilder AddMiddlewares(Action<IConsumerMiddlewareConfigurationBuilder> middlewares)
        {
            middlewares(this.middlewareConfigurationBuilder);
            return this;
        }

        public virtual ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var middlewareConfiguration = this.middlewareConfigurationBuilder.Build();

            var configuration = new ConsumerConfiguration(
                clusterConfiguration,
                this.topics,
                this.groupId,
                this.workersCount,
                this.bufferSize,
                this.distributionStrategyFactory,
                middlewareConfiguration
            )
            {
                AutoOffsetReset = this.autoOffsetReset,
                AutoCommitIntervalMs = this.autoCommitIntervalMs,
                MaxPollIntervalMs = this.maxPollIntervalMs,
                AutoStoreOffsets = this.autoStoreOffsets
            };

            return configuration;
        }
    }
}
