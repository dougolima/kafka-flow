namespace Kafka.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Kafka.Configuration.Consumers;

    public class ConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly ConsumerConfiguration configuration;
        private readonly IMessageConsumer messageConsumer;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private readonly List<IConsumerWorker> workers = new List<IConsumerWorker>();
        private readonly Dictionary<int, IConsumerWorker> partitionWorkers = new Dictionary<int, IConsumerWorker>();

        public ConsumerWorkerPool(
            ConsumerConfiguration configuration,
            IMessageConsumer messageConsumer,
            ILogHandler logHandler,
            IMiddlewareExecutor middlewareExecutor)
        {
            this.configuration = configuration;
            this.messageConsumer = messageConsumer;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
        }

        public async Task StartAsync(
            IConsumer<byte[], byte[]> consumer,
            IReadOnlyCollection<TopicPartition> partitions)
        {
            var workersCount = Math.Min(this.configuration.MaxWorkersCount, partitions.Count);

            for (var i = 0; i < workersCount; i++)
            {
                var worker = new ConsumerWorker(
                    this.configuration.BufferSize,
                    this.messageConsumer,
                    new DefaultOffsetManager(consumer),
                    this.logHandler,
                    this.middlewareExecutor);

                await worker.StartAsync().ConfigureAwait(false);

                this.workers.Add(worker);
            }

            var workerNumber = 0;

            foreach (var partition in partitions)
            {
                this.partitionWorkers.Add(partition.Partition.Value, this.workers[workerNumber]);

                if (++workerNumber >= workersCount)
                {
                    workerNumber = 0;
                }
            }
        }

        public async Task StopAsync()
        {
            await Task.WhenAll(this.workers.Select(x => x.StopAsync())).ConfigureAwait(false);

            this.workers.Clear();
            this.partitionWorkers.Clear();
        }

        public ValueTask EnqueueAsync(ConsumerMessage message)
        {
            return this.partitionWorkers[message.KafkaResult.Partition.Value].EnqueueAsync(message);
        }
    }
}
