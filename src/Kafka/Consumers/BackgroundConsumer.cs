namespace Kafka.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Kafka.Configuration.Consumers;

    public class BackgroundConsumer
    {
        private readonly ConsumerConfiguration configuration;
        private readonly ILogHandler logHandler;
        private readonly IWorkerPool workerPool;

        private readonly ConsumerBuilder<byte[], byte[]> consumerBuilder;

        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundTask;

        public BackgroundConsumer(
            ConsumerConfiguration configuration,
            ILogHandler logHandler,
            IWorkerPool workerPool)
        {
            this.configuration = configuration;
            this.logHandler = logHandler;
            this.workerPool = workerPool;

            var kafkaConfig = configuration.GetKafkaConfig();

            this.consumerBuilder = new ConsumerBuilder<byte[], byte[]>(kafkaConfig);

            this.consumerBuilder.SetPartitionsAssignedHandler(this.OnPartitionAssigned);
            this.consumerBuilder.SetPartitionsRevokedHandler(this.OnPartitionRevoked);
        }

        private void OnPartitionRevoked(IConsumer<byte[], byte[]> consumer, List<TopicPartitionOffset> partitions)
        {
            this.logHandler.Info(
                "Partitions revoked",
                new
                {
                    Topic = this.configuration.Topic,
                    GroupId = this.configuration.GroupId,
                    PartitionCount = partitions.Count,
                    Partitions = partitions.Select(x => x.Partition.Value)
                });

            this.workerPool.StopAsync().GetAwaiter().GetResult();
        }

        private void OnPartitionAssigned(IConsumer<byte[], byte[]> consumer, List<TopicPartition> partitions)
        {
            this.logHandler.Info(
                "Partitions assigned",
                new
                {
                    Topic = this.configuration.Topic,
                    GroupId = this.configuration.GroupId,
                    PartitionCount = partitions.Count,
                    Partitions = partitions.Select(x => x.Partition.Value)
                });

            this.workerPool.StartAsync(consumer, partitions).GetAwaiter().GetResult();
        }

        public Task StartAsync()
        {
            this.cancellationTokenSource = new CancellationTokenSource();

            var consumer = this.consumerBuilder.Build();

            consumer.Subscribe(this.configuration.Topic);

            this.CreateBackgroundTask(consumer);

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            await this.workerPool.StopAsync().ConfigureAwait(false);

            this.cancellationTokenSource.Cancel();
            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
        }

        private void CreateBackgroundTask(IConsumer<byte[], byte[]> consumer)
        {
            this.backgroundTask = Task.Factory.StartNew(
                async () =>
                {
                    using (consumer)
                    {
                        while (!this.cancellationTokenSource.IsCancellationRequested)
                        {
                            ConsumerMessage message = null;

                            try
                            {
                                message = new ConsumerMessage(consumer.Consume(this.cancellationTokenSource.Token));

                                await this.workerPool
                                    .EnqueueAsync(message)
                                    .ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                            }
                            catch (Exception e)
                            {
                                this.logHandler.Error(
                                    "Error consuming message from Kafka",
                                    e,
                                    message);
                            }
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }
    }
}
