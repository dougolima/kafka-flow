namespace Kafka
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Kafka.Configuration;
    using Kafka.Configuration.Consumers;
    using Kafka.Configuration.Consumers.Raw;
    using Kafka.Configuration.Consumers.TypedHandler;
    using Kafka.Consumers;

    public class KafkaBus : IKafkaBus
    {
        private readonly ILogHandler logHandler;
        private readonly IServiceProvider serviceProvider;
        private readonly List<BackgroundConsumer> consumers = new List<BackgroundConsumer>();

        public KafkaBus(
            KafkaConfiguration configuration,
            ILogHandler logHandler,
            IServiceProvider serviceProvider)
        {
            this.logHandler = logHandler;
            this.serviceProvider = serviceProvider;
            this.Configuration = configuration;
        }

        public KafkaConfiguration Configuration { get; }

        public async Task StartAsync()
        {
            foreach (var cluster in this.Configuration.Clusters)
            {
                foreach (var consumerConfiguration in cluster.Consumers)
                {
                    var workerPool = new WorkerPool(
                        consumerConfiguration,
                        this.CreateConsumer(consumerConfiguration),
                        this.logHandler);

                    var consumer = new BackgroundConsumer(
                        consumerConfiguration,
                        this.logHandler,
                        workerPool);

                    this.consumers.Add(consumer);

                    await consumer.StartAsync().ConfigureAwait(false);
                }
            }
        }

        public Task StopAsync()
        {
            return Task.WhenAll(this.consumers.Select(x => x.StopAsync()));
        }

        private IMessageConsumer CreateConsumer(ConsumerConfiguration configuration)
        {
            switch (configuration)
            {
                case TypedHandlerConsumerConfiguration typedConsumerConfiguration:
                    return new TypedHandlerConsumer(
                        typedConsumerConfiguration,
                        this.logHandler,
                        this.serviceProvider);

                case RawConsumerConfiguration rawConsumerConfiguration:
                    return new RawConsumer(rawConsumerConfiguration, this.serviceProvider);

                default:
                    throw new InvalidOperationException($"{configuration.GetType()} not supported");
            }
        }
    }
}
