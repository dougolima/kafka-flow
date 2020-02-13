namespace Kafka.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    public class ConsumerWorker : IConsumerWorker
    {
        private readonly IMessageConsumer consumer;
        private readonly IOffsetManager offsetManager;
        private readonly ILogHandler logHandler;

        private CancellationTokenSource cancellationTokenSource;

        private readonly Channel<ConsumerMessage> messagesBuffer;
        private Task backgroundTask;

        public ConsumerWorker(
            int bufferSize,
            IMessageConsumer consumer,
            IOffsetManager offsetManager,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.logHandler = logHandler;
            this.messagesBuffer = Channel.CreateBounded<ConsumerMessage>(bufferSize);
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
                                await this.consumer
                                    .Cosume(message)
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
                                this.offsetManager.StoreOffset(message.KafkaResult.TopicPartitionOffset);
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
