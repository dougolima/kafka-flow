namespace Kafka.Configuration.Consumers.Raw
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public class RawConsumerConfigurationBuilder : ConsumerConfigurationBuilder<RawConsumerConfigurationBuilder>
    {
        private readonly Type handlerType;

        public RawConsumerConfigurationBuilder(Type handlerType, IServiceCollection services)
            : base(services)
        {
            this.handlerType = handlerType;
        }

        public override ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var baseConfiguration = base.Build(clusterConfiguration);

            var configuration = new RawConsumerConfiguration(
                baseConfiguration,
                this.handlerType);

            return configuration;
        }
    }
}
