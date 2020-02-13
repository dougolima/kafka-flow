namespace Kafka.Configuration.Consumers.Raw
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class RawConsumerConfigurationBuilder : ConsumerConfigurationBuilder<RawConsumerConfigurationBuilder>
    {
        private readonly Type handlerType;
        private readonly IServiceCollection services;

        public RawConsumerConfigurationBuilder(Type handlerType, IServiceCollection services)
            : base(services)
        {
            this.handlerType = handlerType;
            this.services = services;
        }

        public override ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var baseConfiguration = base.Build(clusterConfiguration);

            var configuration = new RawConsumerConfiguration(
                baseConfiguration,
                this.handlerType);

            this.services.TryAddSingleton(configuration.HandlerType);

            return configuration;
        }
    }
}
