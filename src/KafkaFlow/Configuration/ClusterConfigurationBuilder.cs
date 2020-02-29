namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public class ClusterConfigurationBuilder
    {
        private readonly IServiceCollection services;

        private readonly List<ProducerConfigurationBuilder> producers = new List<ProducerConfigurationBuilder>();
        private readonly List<ConsumerConfigurationBuilder> consumers = new List<ConsumerConfigurationBuilder>();

        private IEnumerable<string> brokers;

        public ClusterConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public ClusterConfiguration Build(KafkaConfiguration kafkaConfiguration)
        {
            var configuration = new ClusterConfiguration(
                kafkaConfiguration,
                this.brokers.ToList());

            configuration.AddProducers(this.producers.Select(x => x.Build(configuration)));
            configuration.AddConsumers(this.consumers.Select(x => x.Build(configuration)));

            return configuration;
        }

        /// <summary>
        /// Set the Kafka Brokers to be used
        /// </summary>
        /// <param name="brokers"></param>
        /// <returns></returns>
        public ClusterConfigurationBuilder WithBrokers(IEnumerable<string> brokers)
        {
            this.brokers = brokers;
            return this;
        }

        /// <summary>
        /// Adds a producer to the cluster
        /// </summary>
        /// <param name="producer">A handler to configure the producer</param>
        /// <typeparam name="TProducer">The class responsible for the production</typeparam>
        /// <returns></returns>
        public ClusterConfigurationBuilder AddProducer<TProducer>(Action<IProducerConfigurationBuilder> producer)
        {
            var builder = new ProducerConfigurationBuilder(this.services, typeof(TProducer));

            producer(builder);

            this.producers.Add(builder);

            return this;
        }

        /// <summary>
        /// Adds a consumer to the cluster
        /// </summary>
        /// <param name="consumer">A handler to configure the consumer</param>
        /// <returns></returns>
        public ClusterConfigurationBuilder AddConsumer(Action<IConsumerConfigurationBuilder> consumer)
        {
            var builder = new ConsumerConfigurationBuilder(this.services);

            consumer(builder);

            this.consumers.Add(builder);

            return this;
        }

        //todo: add auth
    }
}
