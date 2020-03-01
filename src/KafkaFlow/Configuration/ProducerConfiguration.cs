namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public class ProducerConfiguration
    {
        public ProducerConfiguration(
            ClusterConfiguration cluster,
            string topic,
            KafkaFlow.Acks? acks,
            IReadOnlyList<Factory<IMessageMiddleware>> middlewaresFactories,
            ProducerConfig baseProducerConfig)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            this.Topic = string.IsNullOrWhiteSpace(topic) ? throw new ArgumentNullException(nameof(topic)) : topic;
            this.Acks = acks;
            this.MiddlewaresFactories = middlewaresFactories;
            this.BaseProducerConfig = baseProducerConfig;
        }

        public IReadOnlyList<Factory<IMessageMiddleware>> MiddlewaresFactories { get; }

        public ClusterConfiguration Cluster { get; }

        public string Topic { get; }

        public ProducerConfig BaseProducerConfig { get; }

        public KafkaFlow.Acks? Acks { get; }

        public ProducerConfig GetKafkaConfig()
        {
            this.BaseProducerConfig.BootstrapServers = string.Join(",", this.Cluster.Brokers);
            this.BaseProducerConfig.Acks = ParseAcks(this.Acks);

            return this.BaseProducerConfig;
        }

        private static Acks? ParseAcks(KafkaFlow.Acks? acks)
        {
            switch (acks)
            {
                case KafkaFlow.Acks.Leader:
                    return Confluent.Kafka.Acks.Leader;

                case KafkaFlow.Acks.All:
                    return Confluent.Kafka.Acks.All;

                case KafkaFlow.Acks.None:
                    return Confluent.Kafka.Acks.None;

                default:
                    return null;
            }
        }
    }
}
