namespace KafkaFlow.Consumers.DistribuitionStrategies
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// This strategy sums all bytes in the partition key and apply a mod operator with the total number of workers, the resulting number is the worker ID to be chosen
    /// This algorithm is fast and creates a good work balance. Messages with the same partition key are always delivered in the same worker, so, message order is guaranteed
    /// Set an optimal message buffer value to avoid idle workers (it will depends how many messages with the same partition key are consumed)
    /// </summary>
    public class BytesSumDistribuitionStrategy : IDistribuitionStrategy
    {
        private IReadOnlyList<IWorker> workers;

        public void Init(IReadOnlyList<IWorker> workers)
        {
            this.workers = workers;
        }

        public Task<IWorker> GetWorkerAsync(byte[] partitionKey)
        {
            return Task.FromResult(this.workers[partitionKey.Sum(x => x) % this.workers.Count]);
        }
    }
}
