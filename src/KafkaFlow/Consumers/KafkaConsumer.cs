﻿namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    public class KafkaConsumer
    {
        private readonly ConsumerConfiguration configuration;
        private readonly ILogHandler logHandler;
        private readonly IConsumerWorkerPool consumerWorkerPool;

        private readonly ConsumerBuilder<byte[], byte[]> consumerBuilder;

        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundTask;

        public KafkaConsumer(
            ConsumerConfiguration configuration,
            ILogHandler logHandler,
            IConsumerWorkerPool consumerWorkerPool)
        {
            this.configuration = configuration;
            this.logHandler = logHandler;
            this.consumerWorkerPool = consumerWorkerPool;

            var kafkaConfig = configuration.GetKafkaConfig();

            this.consumerBuilder = new ConsumerBuilder<byte[], byte[]>(kafkaConfig);

            this.consumerBuilder.SetPartitionsAssignedHandler((consumer, partitions) => this.OnPartitionAssigned(consumer, partitions));
            this.consumerBuilder.SetPartitionsRevokedHandler((consumer, partitions) => this.OnPartitionRevoked(partitions));
        }

        private void OnPartitionRevoked(IReadOnlyCollection<TopicPartitionOffset> partitions)
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

            this.consumerWorkerPool.StopAsync().GetAwaiter().GetResult();
        }

        private void OnPartitionAssigned(IConsumer<byte[], byte[]> consumer, IReadOnlyCollection<TopicPartition> partitions)
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

            this.consumerWorkerPool
                .StartAsync(consumer, partitions, this.cancellationTokenSource.Token)
                .GetAwaiter()
                .GetResult();
        }

        public Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            this.cancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(stopCancellationToken);

            this.CreateBackgroundTask();

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            await this.consumerWorkerPool.StopAsync().ConfigureAwait(false);

            if (this.cancellationTokenSource.Token.CanBeCanceled)
            {
                this.cancellationTokenSource.Cancel();
            }

            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
        }

        private void CreateBackgroundTask()
        {
            var consumer = this.consumerBuilder.Build();

            consumer.Subscribe(this.configuration.Topic);

            this.backgroundTask = Task.Factory.StartNew(
                async () =>
                {
                    using (consumer)
                    {
                        while (!this.cancellationTokenSource.IsCancellationRequested)
                        {
                            try
                            {
                                var message = consumer.Consume(this.cancellationTokenSource.Token);

                                await this.consumerWorkerPool
                                    .EnqueueAsync(message)
                                    .ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                            }
                            catch (KafkaException ex) when (ex.Error.IsFatal)
                            {
                                this.logHandler.Error(
                                    "Kafka fatal error occurred. Trying to restart in 5 seconds",
                                    ex,
                                    null);

                                _ = Task.Delay(5000).ContinueWith(t => this.CreateBackgroundTask());

                                break;
                            }
                            catch (Exception ex)
                            {
                                this.logHandler.Error(
                                    "Error consuming message from Kafka",
                                    ex,
                                    null);
                            }
                        }

                        consumer.Close();
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }
    }
}
