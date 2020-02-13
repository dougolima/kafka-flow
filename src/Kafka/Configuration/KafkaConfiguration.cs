namespace Kafka.Configuration
{
    using System;
    using System.Collections.Generic;

    public class KafkaConfiguration
    {
        public Type LogHandler { get; }
        private readonly List<ClusterConfiguration> clusters = new List<ClusterConfiguration>();

        public KafkaConfiguration(Type logHandler)
        {
            this.LogHandler = logHandler;
        }

        public IReadOnlyCollection<ClusterConfiguration> Clusters => this.clusters;

        public void AddClusters(IEnumerable<ClusterConfiguration> configurations) => this.clusters.AddRange(configurations);
    }
}
