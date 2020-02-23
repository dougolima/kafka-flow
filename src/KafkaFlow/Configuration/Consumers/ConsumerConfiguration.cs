namespace KafkaFlow.Configuration.Consumers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using KafkaFlow.Consumers.DistribuitionStrategies;

    public class ConsumerConfiguration
    {
        public ConsumerConfiguration(
            ClusterConfiguration cluster,
            string topic,
            string groupId,
            int workersCount,
            int bufferSize,
            ConfigurableDefinition<IDistribuitionStrategy> distribuitionStrategy,
            IEnumerable<ConfigurableDefinition<IMessageMiddleware>> middlewares)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            this.DistribuitionStrategy = distribuitionStrategy;
            this.Middlewares = middlewares;
            this.Topic = string.IsNullOrWhiteSpace(topic) ? throw new ArgumentNullException(nameof(topic)) : topic;
            this.GroupId = string.IsNullOrWhiteSpace(groupId) ? throw new ArgumentNullException(nameof(groupId)) : groupId;
            this.WorkersCount = workersCount > 0 ?
                workersCount :
                throw new ArgumentOutOfRangeException(nameof(workersCount), workersCount, "The value must be greater than 0");
            this.BufferSize = bufferSize > 0 ?
                bufferSize :
                throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, "The value must be greater than 0");
        }

        protected ConsumerConfiguration(ConsumerConfiguration baseConfiguration)
        : this(
            baseConfiguration.Cluster,
            baseConfiguration.Topic,
            baseConfiguration.GroupId,
            baseConfiguration.WorkersCount,
            baseConfiguration.BufferSize,
            baseConfiguration.DistribuitionStrategy,
            baseConfiguration.Middlewares)
        {
            this.MaxPollIntervalMs = baseConfiguration.MaxPollIntervalMs;
            this.AutoCommitIntervalMs = baseConfiguration.AutoCommitIntervalMs;
            this.AutoOffsetReset = baseConfiguration.AutoOffsetReset;
            this.AutoStoreOffsets = baseConfiguration.AutoStoreOffsets;
        }

        public ClusterConfiguration Cluster { get; }

        public ConfigurableDefinition<IDistribuitionStrategy> DistribuitionStrategy { get; }

        public IEnumerable<ConfigurableDefinition<IMessageMiddleware>> Middlewares { get; }

        public string Topic { get; }

        public int WorkersCount { get; }

        public string GroupId { get; }

        public int BufferSize { get; }

        public AutoOffsetReset? AutoOffsetReset { get; set; }

        public int? AutoCommitIntervalMs { get; set; }

        public int? MaxPollIntervalMs { get; set; }

        public bool AutoStoreOffsets { get; set; } = true;

        public ConsumerConfig GetKafkaConfig()
        {
            return new ConsumerConfig
            {
                BootstrapServers = string.Join(",", this.Cluster.Brokers),
                AutoOffsetReset = this.AutoOffsetReset,
                GroupId = this.GroupId,
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoCommitIntervalMs = this.AutoCommitIntervalMs,
                MaxPollIntervalMs = this.MaxPollIntervalMs
            };
        }
    }
}
