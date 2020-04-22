namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class ConsumerWorker : IConsumerWorker
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly ConsumerConfiguration configuration;
        private readonly IOffsetManager offsetManager;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private CancellationTokenSource cancellationTokenSource;

        private readonly Channel<ConsumeResult<byte[], byte[]>> messagesBuffer;
        private Task backgroundTask;
        private Action onMessageFinishedHandler;

        public ConsumerWorker(
            IConsumer<byte[], byte[]> consumer,
            int workerId,
            ConsumerConfiguration configuration,
            IOffsetManager offsetManager,
            ILogHandler logHandler,
            IMiddlewareExecutor middlewareExecutor)
        {
            this.Id = workerId;
            this.consumer = consumer;
            this.configuration = configuration;
            this.offsetManager = offsetManager;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
            this.messagesBuffer = Channel.CreateBounded<ConsumeResult<byte[], byte[]>>(configuration.BufferSize);
        }

        public int Id { get; }

        public ValueTask EnqueueAsync(ConsumeResult<byte[], byte[]> message)
        {
            return this.messagesBuffer.Writer.WriteAsync(message);
        }

        public Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            this.cancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(stopCancellationToken);

            this.backgroundTask = Task.Factory.StartNew(
                async () =>
                {
                    while (!this.cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await this.messagesBuffer.Reader
                                .ReadAsync(this.cancellationTokenSource.Token)
                                .ConfigureAwait(false);

                            var context = new ConsumerMessageContext(
                                new MessageContextConsumer(
                                    this.consumer,
                                    this.configuration.ConsumerName,
                                    this.offsetManager,
                                    message),
                                message,
                                this.Id,
                                this.configuration.GroupId);

                            try
                            {
                                await this.middlewareExecutor
                                    .Execute(context, con => Task.CompletedTask)
                                    .ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                this.logHandler.Error(
                                    "Error executing consumer",
                                    ex,
                                    context);
                            }
                            finally
                            {
                                if (this.configuration.AutoStoreOffsets)
                                {
                                    this.offsetManager.StoreOffset(message.TopicPartitionOffset);
                                }

                                this.onMessageFinishedHandler?.Invoke();
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (this.cancellationTokenSource.Token.CanBeCanceled)
            {
                this.cancellationTokenSource.Cancel();
            }

            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
        }

        public void OnTaskCompleted(Action handler)
        {
            this.onMessageFinishedHandler = handler;
        }
    }
}
