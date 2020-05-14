namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using Microsoft.Extensions.DependencyInjection;

    internal class KafkaBus : IKafkaBus
    {
        private readonly KafkaConfiguration configuration;
        private readonly IConsumerManager consumerManager;
        private readonly ILogHandler logHandler;
        private readonly IServiceProvider serviceProvider;
        private readonly IList<KafkaConsumer> consumers = new List<KafkaConsumer>();

        public KafkaBus(
            IConsumerManager consumerManager,
            ILogHandler logHandler,
            IServiceProvider serviceProvider,
            KafkaConfiguration configuration)
        {
            this.configuration = configuration;
            this.consumerManager = consumerManager;
            this.logHandler = logHandler;
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            var consumers = this.configuration.Clusters.SelectMany(cl => cl.Consumers);

            foreach (var consumerConfiguration in consumers)
            {
                var serviceScope = this.serviceProvider.CreateScope();

                var middlewares = consumerConfiguration.MiddlewareConfiguration.Factories
                    .Select(factory => factory(serviceScope.ServiceProvider))
                    .ToList();

                var consumerWorkerPool = new ConsumerWorkerPool(
                    serviceScope.ServiceProvider,
                    consumerConfiguration,
                    this.logHandler,
                    new MiddlewareExecutor(middlewares),
                    consumerConfiguration.DistributionStrategyFactory);

                var consumer = new KafkaConsumer(
                    consumerConfiguration,
                    this.consumerManager,
                    this.logHandler,
                    consumerWorkerPool);

                this.consumers.Add(consumer);

                await consumer.StartAsync(stopCancellationToken).ConfigureAwait(false);
            }
        }

        public Task StopAsync()
        {
            return Task.WhenAll(this.consumers.Select(x => x.StopAsync()));
        }
    }
}
