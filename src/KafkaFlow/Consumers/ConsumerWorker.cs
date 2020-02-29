namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal class ConsumerWorker : IConsumerWorker
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConsumerConfiguration configuration;
        private readonly IOffsetManager offsetManager;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private CancellationTokenSource cancellationTokenSource;

        private readonly Channel<ConsumerMessage> messagesBuffer;
        private Task backgroundTask;
        private Action onMessageFinishedHandler;

        public ConsumerWorker(
            IServiceProvider serviceProvider,
            int workerId,
            ConsumerConfiguration configuration,
            IOffsetManager offsetManager,
            ILogHandler logHandler,
            IMiddlewareExecutor middlewareExecutor)
        {
            this.Id = workerId;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
            this.offsetManager = offsetManager;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
            this.messagesBuffer = Channel.CreateBounded<ConsumerMessage>(configuration.BufferSize);
        }

        public int Id { get; }

        public ValueTask EnqueueAsync(ConsumerMessage message)
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

                            try
                            {
                                var context = new MessageContext(
                                    message,
                                    this.offsetManager,
                                    this.Id,
                                    this.configuration.SerializerFactory(this.serviceProvider),
                                    this.configuration.CompressorFactory(this.serviceProvider));

                                await this.middlewareExecutor
                                    .Execute(context, con => Task.CompletedTask)
                                    .ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                this.logHandler.Error("Error executing consumer", ex, message);
                            }
                            finally
                            {
                                if (this.configuration.AutoStoreOffsets)
                                {
                                    this.offsetManager.StoreOffset(message.KafkaResult.TopicPartitionOffset);
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
