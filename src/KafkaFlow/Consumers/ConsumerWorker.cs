namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration.Consumers;

    public class ConsumerWorker : IConsumerWorker
    {
        private readonly ConsumerConfiguration configuration;
        private readonly IMessageConsumer consumer;
        private readonly IOffsetManager offsetManager;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private CancellationTokenSource cancellationTokenSource;

        private readonly Channel<ConsumerMessage> messagesBuffer;
        private Task backgroundTask;

        public ConsumerWorker(
            ConsumerConfiguration configuration,
            IMessageConsumer consumer,
            IOffsetManager offsetManager,
            ILogHandler logHandler,
            IMiddlewareExecutor middlewareExecutor)
        {
            this.configuration = configuration;
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
            this.messagesBuffer = Channel.CreateBounded<ConsumerMessage>(configuration.BufferSize);
        }

        public ValueTask EnqueueAsync(ConsumerMessage message)
        {
            return this.messagesBuffer.Writer.WriteAsync(message);
        }

        public Task StartAsync()
        {
            this.cancellationTokenSource = new CancellationTokenSource();

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
                                var context = this.consumer.CreateMessageContext(message, this.offsetManager);

                                await this.middlewareExecutor
                                    .Execute(context, this.consumer.Cosume)
                                    .ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                this.logHandler.Error(
                                    "Error executing consumer",
                                    ex,
                                    message);
                            }
                            finally
                            {
                                if (this.configuration.AutoStoreOffsets)
                                {
                                    this.offsetManager.StoreOffset(message.KafkaResult.TopicPartitionOffset);
                                }
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
            this.cancellationTokenSource.Cancel();
            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
        }
    }
}
