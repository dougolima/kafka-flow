namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    public class KafkaConfiguration
    {
        private readonly List<ClusterConfiguration> clusters = new List<ClusterConfiguration>();

        public IReadOnlyCollection<ClusterConfiguration> Clusters => this.clusters;

        public void AddClusters(IEnumerable<ClusterConfiguration> configurations) => this.clusters.AddRange(configurations);
    }
}
