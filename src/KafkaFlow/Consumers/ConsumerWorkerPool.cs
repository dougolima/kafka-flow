namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration.Consumers;
    using KafkaFlow.Consumers.DistribuitionStrategies;

    public class ConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly ConsumerConfiguration configuration;
        private readonly IMessageConsumer messageConsumer;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly IDistribuitionStrategyFactory distribuitionStrategyFactory;

        private readonly List<IConsumerWorker> workers = new List<IConsumerWorker>();

        private IDistribuitionStrategy distribuitionStrategy;

        private OffsetManager offsetManager;

        public ConsumerWorkerPool(
            ConsumerConfiguration configuration,
            IMessageConsumer messageConsumer,
            ILogHandler logHandler,
            IMiddlewareExecutor middlewareExecutor,
            IDistribuitionStrategyFactory distribuitionStrategyFactory)
        {
            this.configuration = configuration;
            this.messageConsumer = messageConsumer;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
            this.distribuitionStrategyFactory = distribuitionStrategyFactory;
        }

        public async Task StartAsync(
            IConsumer<byte[], byte[]> consumer,
            IReadOnlyCollection<TopicPartition> partitions)
        {
            this.offsetManager = new OffsetManager(consumer, partitions);
            var workersCount = this.configuration.WorkersCount;

            await Task.WhenAll(Enumerable
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
                }))
                .ConfigureAwait(false);

            this.distribuitionStrategy = this.distribuitionStrategyFactory.Create();
            this.distribuitionStrategy.Init(this.workers.AsReadOnly());
        }

        public async Task StopAsync()
        {
            await Task.WhenAll(this.workers.Select(x => x.StopAsync())).ConfigureAwait(false);

            this.workers.Clear();
        }

        public async Task EnqueueAsync(ConsumerMessage message)
        {
            this.offsetManager.InitializeOffsetIfNeeded(message);

            var worker = (IConsumerWorker)await this.distribuitionStrategy
                .GetWorkerAsync(message.Key)
                .ConfigureAwait(false);

            await worker
                .EnqueueAsync(message)
                .ConfigureAwait(false);
        }
    }
}
