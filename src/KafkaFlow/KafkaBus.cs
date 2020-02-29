namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;

    public class KafkaBus : IKafkaBus
    {
        private readonly ILogHandler logHandler;
        private readonly IServiceProvider serviceProvider;
        private readonly List<KafkaConsumer> consumers = new List<KafkaConsumer>();

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

        public async Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            foreach (var cluster in this.Configuration.Clusters)
            {
                foreach (var consumerConfiguration in cluster.Consumers)
                {
                    var consumerWorkerPool = new ConsumerWorkerPool(
                        this.serviceProvider,
                        consumerConfiguration,
                        this.logHandler,
                        new MiddlewareExecutor(consumerConfiguration.MiddlewaresFactories, this.serviceProvider),
                        consumerConfiguration.DistribuitionStrategyFactory);

                    var consumer = new KafkaConsumer(
                        consumerConfiguration,
                        this.logHandler,
                        consumerWorkerPool);

                    this.consumers.Add(consumer);

                    await consumer.StartAsync(stopCancellationToken).ConfigureAwait(false);
                }
            }
        }

        public Task StopAsync()
        {
            return Task.WhenAll(this.consumers.Select(x => x.StopAsync()));
        }
    }
}
