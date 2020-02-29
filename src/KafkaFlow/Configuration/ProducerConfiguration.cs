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
            Factory<IMessageSerializer> serializerFactory,
            Factory<IMessageCompressor> compressorFactory,
            KafkaFlow.Acks? acks,
            IReadOnlyList<Factory<IMessageMiddleware>> middlewaresFactories,
            ProducerConfig baseProducerConfig)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            this.Topic = string.IsNullOrWhiteSpace(topic) ? throw new ArgumentNullException(nameof(topic)) : topic;
            this.SerializerFactory = serializerFactory ?? throw new ArgumentNullException(nameof(serializerFactory));
            this.CompressorFactory = compressorFactory ?? throw new ArgumentNullException(nameof(compressorFactory));
            this.Acks = acks;
            this.MiddlewaresFactories = middlewaresFactories;
            this.BaseProducerConfig = baseProducerConfig;
        }

        public IReadOnlyList<Factory<IMessageMiddleware>> MiddlewaresFactories { get; }

        public Factory<IMessageCompressor> CompressorFactory { get; }

        public Factory<IMessageSerializer> SerializerFactory { get; }

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
            return acks switch
            {
                KafkaFlow.Acks.Leader => Confluent.Kafka.Acks.Leader,
                KafkaFlow.Acks.All => Confluent.Kafka.Acks.All,
                KafkaFlow.Acks.None => Confluent.Kafka.Acks.None,
                _ => null
            };
        }
    }
}
