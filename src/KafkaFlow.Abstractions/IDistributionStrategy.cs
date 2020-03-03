namespace KafkaFlow
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDistributionStrategy
    {
        void Init(IReadOnlyList<IWorker> workers);

        Task<IWorker> GetWorkerAsync(byte[] partitionKey);
    }
}
