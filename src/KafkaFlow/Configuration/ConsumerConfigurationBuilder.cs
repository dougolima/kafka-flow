namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using KafkaFlow.Consumers.DistributionStrategies;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    internal sealed class ConsumerConfigurationBuilder
        : IConsumerConfigurationBuilder
    {
        private readonly List<string> topics = new List<string>();
        private readonly ConsumerMiddlewareConfigurationBuilder middlewareConfigurationBuilder;

        private ConsumerConfig consumerConfig;

        private string name;
        private int workersCount;
        private int bufferSize;
        private bool autoStoreOffsets = true;

        private Factory<IDistributionStrategy> distributionStrategyFactory = provider => new BytesSumDistributionStrategy();


        public ConsumerConfigurationBuilder(IServiceCollection services)
        {
            this.middlewareConfigurationBuilder = new ConsumerMiddlewareConfigurationBuilder(services);
            this.consumerConfig = new ConsumerConfig();
            this.ServiceCollection = services;
        }

        public IServiceCollection ServiceCollection { get; }

        public IConsumerConfigurationBuilder Topic(string topic)
        {
            this.topics.Add(topic);
            return this;
        }

        public IConsumerConfigurationBuilder WithConsumerConfig(ConsumerConfig config)
        {
            this.consumerConfig = config;
            return this;
        }

        public IConsumerConfigurationBuilder Topics(IEnumerable<string> topics)
        {
            this.topics.AddRange(topics);
            return this;
        }

        public IConsumerConfigurationBuilder Topics(params string[] topics) => this.Topics(topics.AsEnumerable());

        public IConsumerConfigurationBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public IConsumerConfigurationBuilder WithGroupId(string groupId)
        {
            this.consumerConfig.GroupId = groupId;
            return this;
        }

        public IConsumerConfigurationBuilder WithAutoOffsetReset(KafkaFlow.AutoOffsetReset autoOffsetReset)
        {
            switch (autoOffsetReset)
            {
                case KafkaFlow.AutoOffsetReset.Earliest:
                    this.consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                    break;
                case KafkaFlow.AutoOffsetReset.Latest:
                    this.consumerConfig.AutoOffsetReset = AutoOffsetReset.Latest;
                    break;
                default: break;
            }

            return this;
        }

        public IConsumerConfigurationBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs)
        {
            this.consumerConfig.AutoCommitIntervalMs = autoCommitIntervalMs;
            return this;
        }

        public IConsumerConfigurationBuilder WithMaxPollIntervalMs(int maxPollIntervalMs)
        {
            this.consumerConfig.MaxPollIntervalMs = maxPollIntervalMs;
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

        public ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var middlewareConfiguration = this.middlewareConfigurationBuilder.Build();

            this.consumerConfig.BootstrapServers = string.Join(",", clusterConfiguration.Brokers);
            this.consumerConfig.EnableAutoOffsetStore = false;
            this.consumerConfig.EnableAutoCommit = true;

            return new ConsumerConfiguration(
                this.consumerConfig,
                this.topics,
                this.name,
                this.workersCount,
                this.bufferSize,
                this.distributionStrategyFactory,
                middlewareConfiguration
            )
            {
                AutoStoreOffsets = this.autoStoreOffsets
            };
        }
    }
}
