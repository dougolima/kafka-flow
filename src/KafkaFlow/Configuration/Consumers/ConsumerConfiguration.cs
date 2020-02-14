namespace KafkaFlow.Configuration.Consumers
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
            int maxWorkersCount,
            int bufferSize,
            IEnumerable<MiddlewareDefinition> middlewares)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            this.Middlewares = middlewares;
            this.Topic = string.IsNullOrWhiteSpace(topic) ? throw new ArgumentNullException(nameof(topic)) : topic;
            this.GroupId = string.IsNullOrWhiteSpace(groupId) ? throw new ArgumentNullException(nameof(groupId)) : groupId;
            this.MaxWorkersCount = maxWorkersCount > 0 ?
                maxWorkersCount :
                throw new ArgumentOutOfRangeException(nameof(maxWorkersCount), maxWorkersCount, "The value must be greater than 0");
            this.BufferSize = bufferSize > 0 ?
                bufferSize :
                throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, "The value must be greater than 0");
        }

        protected ConsumerConfiguration(ConsumerConfiguration baseConfiguration)
        : this(
            baseConfiguration.Cluster,
            baseConfiguration.Topic,
            baseConfiguration.GroupId,
            baseConfiguration.MaxWorkersCount,
            baseConfiguration.BufferSize,
            baseConfiguration.Middlewares)
        {
            this.MaxPollIntervalMs = baseConfiguration.MaxPollIntervalMs;
            this.AutoCommitIntervalMs = baseConfiguration.AutoCommitIntervalMs;
            this.AutoOffsetReset = baseConfiguration.AutoOffsetReset;
        }

        public ClusterConfiguration Cluster { get; }

        public IEnumerable<MiddlewareDefinition> Middlewares { get; }

        public string Topic { get; }

        public int MaxWorkersCount { get; }

        public string GroupId { get; }

        public int BufferSize { get; }

        public AutoOffsetReset? AutoOffsetReset { get; set; }

        public int? AutoCommitIntervalMs { get; set; }

        public int? MaxPollIntervalMs { get; set; }

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
