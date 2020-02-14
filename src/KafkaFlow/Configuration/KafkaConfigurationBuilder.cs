namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class KafkaConfigurationBuilder
    {
        private readonly IServiceCollection services;
        private readonly List<ClusterConfigurationBuilder> clusters = new List<ClusterConfigurationBuilder>();
        private Type logHandler;

        public KafkaConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public KafkaConfiguration Build()
        {
            var configuration = new KafkaConfiguration(this.logHandler);

            configuration.AddClusters(this.clusters.Select(x => x.Build(configuration)));

            this.services.TryAddSingleton(typeof(ILogHandler), this.logHandler);

            return configuration;
        }

        public KafkaConfigurationBuilder AddCluster(Action<ClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.services);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }

        public KafkaConfigurationBuilder UseLogHandler<TLogHandler>() where TLogHandler : ILogHandler
        {
            this.logHandler = typeof(TLogHandler);
            return this;
        }
    }
}
