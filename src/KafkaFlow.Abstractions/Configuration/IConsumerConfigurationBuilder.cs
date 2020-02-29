namespace KafkaFlow.Configuration
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IConsumerConfigurationBuilder
    {
        IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// Set the topic that will be used to read the messages
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder Topic(string topic);

        /// <summary>
        /// Set the group id used by the consumer
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithGroupId(string groupId);

        /// <summary>
        /// Set the initial offset strategy used by new consumer groups.
        /// If your consumer group (set by method <see cref="KafkaFlow.Configuration.Consumers.ConsumerConfigurationBuilder.WithGroupId(string)"/>) has no offset stored in Kafka, this configuration will be used
        /// Use Earliest to read the topic from the beginning
        /// Use Latest to read only new messages in the topic
        /// </summary>
        /// <param name="autoOffsetReset"></param>
        /// <returns></returns>
        // IConsumerConfigurationBuilder WithAutoOffsetReset(AutoOffsetReset autoOffsetReset);

        /// <summary>
        /// Set the interval used by the framework to commit the stored offsets in Kafka
        /// </summary>
        /// <param name="autoCommitIntervalMs">The interval in milliseconds</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs);

        /// <summary>
        /// Set the max interval between message consumption, if this time exceeds the consumer is considered failed and Kafka will revoke the assigned partitions
        /// </summary>
        /// <param name="maxPollIntervalMs">The interval in milliseconds</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithMaxPollIntervalMs(int maxPollIntervalMs);

        /// <summary>
        /// Set the number of threads that will be used to consume the messages
        /// </summary>
        /// <param name="workersCount"></param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithWorkersCount(int workersCount);

        /// <summary>
        /// Set how many messages will be buffered for each worker
        /// </summary>
        /// <param name="size">The buffer size</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithBufferSize(int size);

        /// <summary>
        /// Set the strategy to choose a worker when a message arrives
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IDistribuitionStrategy"/> interface</typeparam>
        /// <param name="factory">A factory to create the instance</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithWorkDistribuitionStretagy<T>(Factory<T> factory)
            where T : IDistribuitionStrategy;

        /// <summary>
        /// Set the strategy to choose a worker when a message arrives
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IDistribuitionStrategy"/> interface</typeparam>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithWorkDistribuitionStretagy<T>()
            where T : IDistribuitionStrategy;

        /// <summary>
        /// Offsets will be stored after the execution of the handler and middlewares automatically, this is the default behaviour
        /// </summary>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithAutoStoreOffsets();

        /// <summary>
        /// The Handler or Middleware should call the <see cref="KafkaFlow.IMessageContext.StoreOffset()">
        /// </summary>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithManualStoreOffsets();

        /// <summary>
        /// Register a middleware to be used when consuming messages
        /// </summary>
        /// <param name="factory">A factory to create the instance</param>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IConsumerConfigurationBuilder UseMiddleware<T>(Factory<T> factory)
            where T : IMessageMiddleware;

        /// <summary>
        /// Register a middleware to be used when consuming messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IConsumerConfigurationBuilder UseMiddleware<T>()
            where T : IMessageMiddleware;

        /// <summary>
        /// Set the default serializer to be used when consuming messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageSerializer"/> interface</typeparam>
        /// <returns></returns>
        IConsumerConfigurationBuilder UseSerializer<T>()
            where T : IMessageSerializer;

        /// <summary>
        /// Set the default serializer to be used when consuming messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageSerializer"/> interface</typeparam>
        /// <returns></returns>
        IConsumerConfigurationBuilder UseSerializer<T>(Factory<T> factory)
            where T : IMessageSerializer;

        /// <summary>
        /// Set the default compressor to be used when consuming messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageCompressor"/> interface</typeparam>
        /// <returns></returns>
        IConsumerConfigurationBuilder UseCompressor<T>()
            where T : IMessageCompressor;

        /// <summary>
        /// Set the default compressor to be used when consuming messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageCompressor"/> interface</typeparam>
        /// <returns></returns>
        IConsumerConfigurationBuilder UseCompressor<T>(Factory<T> factory)
            where T : IMessageCompressor;
    }
}
