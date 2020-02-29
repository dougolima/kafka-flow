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
            Acks? acks,
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

        public Acks? Acks { get; }

        public ProducerConfig GetKafkaConfig()
        {
            this.BaseProducerConfig.BootstrapServers = string.Join(",", this.Cluster.Brokers);
            this.BaseProducerConfig.Acks = this.Acks;

            return this.BaseProducerConfig;
        }
    }
}
