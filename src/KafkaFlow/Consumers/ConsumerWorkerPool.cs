namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration.Consumers;

    public class ConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly ConsumerConfiguration configuration;
        private readonly IMessageConsumer messageConsumer;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private readonly List<IConsumerWorker> workers = new List<IConsumerWorker>();

        private readonly IWorkerDistribuitionStrategy distribuitionStrategy = new SumWorkerDistribuitionStrategy();
        private OffsetManager offsetManager;

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

        public Task StartAsync(
            IConsumer<byte[], byte[]> consumer,
            IReadOnlyCollection<TopicPartition> partitions)
        {
            this.offsetManager = new OffsetManager(consumer, partitions);
            var workersCount = this.configuration.WorkersCount;

            return Task.WhenAll(Enumerable
                .Range(0, workersCount)
                .Select(workerId =>
                {
                    var worker = new ConsumerWorker(
                        workerId,
                        this.configuration,
                        this.messageConsumer, this.offsetManager,
                        this.logHandler,
                        this.middlewareExecutor);

                    this.workers.Add(worker);

                    return worker.StartAsync();
                }));
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
