namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;
    using Confluent.Kafka;
    using KafkaFlow.Consumers.DistribuitionStrategies;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class ConsumerConfigurationBuilder
        : IConsumerConfigurationBuilder
    {

        private string topic;
        private string groupId;
        private AutoOffsetReset? autoOffsetReset;
        private int? autoCommitIntervalMs;
        private int? maxPollIntervalMs;
        private int workersCount;
        private int bufferSize;
        private bool? autoStoreOffsets;

        private Factory<IDistribuitionStrategy> distribuitionStrategyFactory = provider => new BytesSumDistribuitionStrategy();

        private readonly List<Factory<IMessageMiddleware>> middlewaresFactories = new List<Factory<IMessageMiddleware>>();

        private Factory<IMessageSerializer> serializerFactory;

        private Factory<IMessageCompressor> compressorFactory = provider => new NullMessageCompressor();

        public ConsumerConfigurationBuilder(IServiceCollection services)
        {
            this.ServiceCollection = services;
        }

        public IServiceCollection ServiceCollection { get; }

        public IConsumerConfigurationBuilder Topic(string topic)
        {
            this.topic = topic;
            return this;
        }

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

        public IConsumerConfigurationBuilder WithWorkDistribuitionStretagy<T>(Factory<T> factory)
            where T : IDistribuitionStrategy
        {
            this.distribuitionStrategyFactory = provider => factory(provider);
            return this;
        }

        public IConsumerConfigurationBuilder WithWorkDistribuitionStretagy<T>()
            where T : IDistribuitionStrategy
        {
            this.distribuitionStrategyFactory = provider => provider.GetRequiredService<T>();
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

        public IConsumerConfigurationBuilder UseMiddleware<T>()
            where T : IMessageMiddleware
        {
            return this.UseMiddleware(provider => provider.GetRequiredService<T>());
        }

        public IConsumerConfigurationBuilder UseMiddleware<T>(Factory<T> factory)
            where T : IMessageMiddleware
        {
            this.ServiceCollection.TryAddSingleton(typeof(T));
            this.middlewaresFactories.Add(provider => factory(provider));
            return this;
        }

        public IConsumerConfigurationBuilder UseSerializer<T>()
            where T : IMessageSerializer
        {
            return this.UseSerializer(provider => provider.GetRequiredService<T>());

        }

        public IConsumerConfigurationBuilder UseSerializer<T>(Factory<T> factory)
            where T : IMessageSerializer
        {
            this.ServiceCollection.TryAddSingleton(typeof(T));
            this.serializerFactory = provider => factory(provider);
            return this;
        }

        public IConsumerConfigurationBuilder UseCompressor<T>()
            where T : IMessageCompressor
        {
            return this.UseCompressor(provider => provider.GetRequiredService<T>());
        }

        public IConsumerConfigurationBuilder UseCompressor<T>(Factory<T> factory)
            where T : IMessageCompressor
        {
            this.ServiceCollection.TryAddSingleton(typeof(T));
            this.compressorFactory = provider => factory(provider);
            return this;
        }

        public virtual ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var configuration = new ConsumerConfiguration(
                clusterConfiguration,
                this.topic,
                this.groupId,
                this.workersCount,
                this.bufferSize,
                this.distribuitionStrategyFactory,
                this.middlewaresFactories,
                this.serializerFactory,
                this.compressorFactory)
            {
                AutoOffsetReset = this.autoOffsetReset,
                AutoCommitIntervalMs = this.autoCommitIntervalMs,
                MaxPollIntervalMs = this.maxPollIntervalMs
            };

            configuration.AutoStoreOffsets = this.autoStoreOffsets ?? configuration.AutoStoreOffsets;

            return configuration;
        }
    }
}
