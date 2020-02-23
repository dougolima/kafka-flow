namespace KafkaFlow.Configuration.Producers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public class ProducerConfiguration
    {
        public ProducerConfiguration(ClusterConfiguration cluster,
            string topic,
            Type serializer,
            Type compressor,
            Acks? acks,
            IEnumerable<ConfigurableDefinition<IMessageMiddleware>> middlewares,
            ProducerConfig baseProducerConfig)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            this.Topic = string.IsNullOrWhiteSpace(topic) ? throw new ArgumentNullException(nameof(topic)) : topic;
            this.Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.Compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            this.Acks = acks;
            this.Middlewares = middlewares;
            this.BaseProducerConfig = baseProducerConfig;
        }

        public ClusterConfiguration Cluster { get; }

        public string Topic { get; }

        public Type Serializer { get; }

        public Type Compressor { get; }

        public ProducerConfig BaseProducerConfig { get; }

        public Acks? Acks { get; }

        public IEnumerable<ConfigurableDefinition<IMessageMiddleware>> Middlewares { get; }

        public ProducerConfig GetKafkaConfig()
        {
            this.BaseProducerConfig.BootstrapServers = string.Join(",", this.Cluster.Brokers);
            this.BaseProducerConfig.Acks = this.Acks;

            return this.BaseProducerConfig;
        }
    }
}
