namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

    internal class ConsumerConfiguration
    {
        public ConsumerConfiguration(
            ClusterConfiguration cluster,
            IEnumerable<string> topics,
            string groupId,
            int workersCount,
            int bufferSize,
            Factory<IDistributionStrategy> distributionStrategyFactory,
            MiddlewareConfiguration middlewareConfiguration)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            this.DistributionStrategyFactory = distributionStrategyFactory ?? throw new ArgumentNullException(nameof(distributionStrategyFactory));
            this.MiddlewareConfiguration = middlewareConfiguration ?? throw new ArgumentNullException(nameof(middlewareConfiguration));
            this.Topics = topics ?? throw new ArgumentNullException(nameof(topics));
            this.GroupId = string.IsNullOrWhiteSpace(groupId) ? throw new ArgumentNullException(nameof(groupId)) : groupId;
            this.WorkersCount = workersCount > 0 ?
                workersCount :
                throw new ArgumentOutOfRangeException(
                    nameof(workersCount),
                    workersCount,
                    "The value must be greater than 0");
            this.BufferSize = bufferSize > 0 ?
                bufferSize :
                throw new ArgumentOutOfRangeException(
                    nameof(bufferSize),
                    bufferSize,
                    "The value must be greater than 0");
        }

        public ClusterConfiguration Cluster { get; }

        public Factory<IDistributionStrategy> DistributionStrategyFactory { get; }

        public MiddlewareConfiguration MiddlewareConfiguration { get; }

        public IEnumerable<string> Topics { get; }

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
                AutoOffsetReset = ParseAutoOffsetReset(this.AutoOffsetReset),
                GroupId = this.GroupId,
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoCommitIntervalMs = this.AutoCommitIntervalMs,
                MaxPollIntervalMs = this.MaxPollIntervalMs
            };
        }

        private static Confluent.Kafka.AutoOffsetReset? ParseAutoOffsetReset(AutoOffsetReset? autoOffsetReset)
        {
            switch (autoOffsetReset)
            {
                case KafkaFlow.AutoOffsetReset.Latest:
                    return Confluent.Kafka.AutoOffsetReset.Latest;

                case KafkaFlow.AutoOffsetReset.Earliest:
                    return Confluent.Kafka.AutoOffsetReset.Latest;

                default:
                    return null;
            }
        }
    }
}
