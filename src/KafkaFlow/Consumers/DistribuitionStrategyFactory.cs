namespace KafkaFlow.Consumers
{
    using System;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers.DistribuitionStrategies;
    using Microsoft.Extensions.DependencyInjection;

    internal class DistribuitionStrategyFactory:IDistribuitionStrategyFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConfigurableDefinition<IDistribuitionStrategy> definition;

        public DistribuitionStrategyFactory(
            IServiceProvider serviceProvider,
            ConfigurableDefinition<IDistribuitionStrategy> definition)
        {
            this.serviceProvider = serviceProvider;
            this.definition = definition;
        }

        public IDistribuitionStrategy Create()
        {
            var strategy = (IDistribuitionStrategy)this.serviceProvider.GetRequiredService(this.definition.Type);

            this.definition.Configurator(strategy,this.serviceProvider);

            return strategy;
        }
    }
}