namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Configuration.Consumers;
    using KafkaFlow.Configuration.Producers;

    public class ClusterConfiguration
    {
        private readonly List<ProducerConfiguration> producers = new List<ProducerConfiguration>();
        private readonly List<ConsumerConfiguration> consumers = new List<ConsumerConfiguration>();

        public ClusterConfiguration(
            KafkaConfiguration kafka,
            IReadOnlyCollection<string> brokers,
            IReadOnlyCollection<ConfigurableDefinition<IMessageMiddleware>> consumersMiddlewares,
            IReadOnlyCollection<ConfigurableDefinition<IMessageMiddleware>> producersMiddlewares)
        {
            this.Kafka = kafka;
            this.ConsumersMiddlewares = consumersMiddlewares;
            this.ProducersMiddlewares = producersMiddlewares;
            this.Brokers = brokers.ToList();
        }

        public KafkaConfiguration Kafka { get; }

        public IReadOnlyCollection<ConfigurableDefinition<IMessageMiddleware>> ConsumersMiddlewares { get; }

        public IReadOnlyCollection<ConfigurableDefinition<IMessageMiddleware>> ProducersMiddlewares { get; }

        public IReadOnlyCollection<string> Brokers { get; }

        public IReadOnlyCollection<ProducerConfiguration> Producers => this.producers;

        public IReadOnlyCollection<ConsumerConfiguration> Consumers => this.consumers;

        public void AddConsumers(IEnumerable<ConsumerConfiguration> configurations) => this.consumers.AddRange(configurations);

        public void AddProducers(IEnumerable<ProducerConfiguration> configurations) => this.producers.AddRange(configurations);
    }
}
