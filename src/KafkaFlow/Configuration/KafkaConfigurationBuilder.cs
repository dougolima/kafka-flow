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
        private Type logHandler = typeof(NullLogHandler);

        public KafkaConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public KafkaConfiguration Build()
        {
            var configuration = new KafkaConfiguration();

            configuration.AddClusters(this.clusters.Select(x => x.Build(configuration)));

            this.services.TryAddSingleton(typeof(ILogHandler), this.logHandler);

            return configuration;
        }

        /// <summary>
        /// Adds a new Cluster
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        public KafkaConfigurationBuilder AddCluster(Action<ClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.services);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }

        /// <summary>
        /// Set the log handler to be used by the Framework, if none is provided the <see cref="NullLogHandler"/> will be used
        /// </summary>
        /// <typeparam name="TLogHandler">A class that implements the <see cref="ILogHandler"/> interface</typeparam>
        /// <returns></returns>
        public KafkaConfigurationBuilder UseLogHandler<TLogHandler>() where TLogHandler : ILogHandler
        {
            this.logHandler = typeof(TLogHandler);
            return this;
        }
    }
}
