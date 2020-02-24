namespace KafkaFlow.Consumers.DistribuitionStrategies
{
    using System.Collections.Generic;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    /// <summary>
    /// This strategy chooses the first free worker to process the message. When a worker finishes the processing, it notifies the worker pool that it is free to get a new message
    /// This is the fastest and resource-friendly strategy (the message buffer is not used) but messages with the same partition key can be delivered in different workers, so, no message order guarantee
    /// </summary>
    public class FreeWorkerDistribuitionStrategy : IDistribuitionStrategy
    {
        private readonly Channel<IWorker> freeWorkers = Channel.CreateUnbounded<IWorker>();

        public void Init(IReadOnlyList<IWorker> workers)
        {
            foreach (var worker in workers)
            {
                worker.OnTaskFinished(() => this.freeWorkers.Writer.WriteAsync(worker));
                this.freeWorkers.Writer.TryWrite(worker);
            }
        }

        public Task<IWorker> GetWorkerAsync(byte[] partitionKey)
        {
            return this.freeWorkers.Reader.ReadAsync().AsTask();
        }
    }
}
