namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public class ConsumerConfiguration
    {
        public ConsumerConfiguration(
            ClusterConfiguration cluster,
            string topic,
            string groupId,
            int workersCount,
            int bufferSize,
            Factory<IDistribuitionStrategy> distribuitionStrategyFactory,
            List<Factory<IMessageMiddleware>> middlewaresFactories)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            this.DistribuitionStrategyFactory = distribuitionStrategyFactory ?? throw new ArgumentNullException(nameof(distribuitionStrategyFactory));
            this.MiddlewaresFactories = middlewaresFactories ?? throw new ArgumentNullException(nameof(middlewaresFactories));
            this.Topic = string.IsNullOrWhiteSpace(topic) ? throw new ArgumentNullException(nameof(topic)) : topic;
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

        public Factory<IDistribuitionStrategy> DistribuitionStrategyFactory { get; }

        public List<Factory<IMessageMiddleware>> MiddlewaresFactories { get; }

        public string Topic { get; }

        public int WorkersCount { get; }

        public string GroupId { get; }

        public int BufferSize { get; }

        public KafkaFlow.AutoOffsetReset? AutoOffsetReset { get; set; }

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

        private static AutoOffsetReset? ParseAutoOffsetReset(KafkaFlow.AutoOffsetReset? autoOffsetReset)
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
