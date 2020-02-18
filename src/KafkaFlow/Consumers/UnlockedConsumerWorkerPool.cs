namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration.Consumers;

    public class UnlockedConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly ConsumerConfiguration configuration;
        private readonly IMessageConsumer messageConsumer;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private readonly List<IConsumerWorker> workers = new List<IConsumerWorker>();

        private readonly IWorkerDistribuitionStrategy distribuitionStrategy = new SumWorkerDistribuitionStrategy();
        private UnlockedOffsetManager offsetManager;

        public UnlockedConsumerWorkerPool(
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
            this.offsetManager = new UnlockedOffsetManager(consumer, partitions);
            var workersCount = this.configuration.WorkersCount;

            for (var i = 0; i < workersCount; i++)
            {
                var worker = new ConsumerWorker(
                    this.configuration,
                    this.messageConsumer, this.offsetManager,
                    this.logHandler,
                    this.middlewareExecutor);

                await worker.StartAsync().ConfigureAwait(false);

                this.workers.Add(worker);
            }
        }

        public async Task StopAsync()
        {
            await Task.WhenAll(this.workers.Select(x => x.StopAsync())).ConfigureAwait(false);

            this.workers.Clear();
        }

        public ValueTask EnqueueAsync(ConsumerMessage message)
        {
            this.offsetManager.InitializeOffsetIfNeeded(message);

            var workerNumber = this.distribuitionStrategy.Distribute(message.Key, this.workers.Count);

            return this.workers[workerNumber].EnqueueAsync(message);
        }
    }
}
