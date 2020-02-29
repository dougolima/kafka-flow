namespace KafkaFlow.Configuration
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IProducerConfigurationBuilder
    {
        IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// Register a middleware to be used when producing messages
        /// </summary>
        /// <param name="factory">A factory to create the instance</param>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IProducerConfigurationBuilder UseMiddleware<T>(Factory<T> factory)
            where T : IMessageMiddleware;

        /// <summary>
        /// Register a middleware to be used when producing messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IProducerConfigurationBuilder UseMiddleware<T>()
            where T : IMessageMiddleware;

        /// <summary>
        /// Set the default serializer to be used when producing messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageSerializer"/> interface</typeparam>
        /// <returns></returns>
        IProducerConfigurationBuilder UseSerializer<T>()
            where T : IMessageSerializer;

        /// <summary>
        /// Set the default serializer to be used when producing messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageSerializer"/> interface</typeparam>
        /// <returns></returns>
        IProducerConfigurationBuilder UseSerializer<T>(Factory<T> factory)
            where T : IMessageSerializer;

        /// <summary>
        /// Set the default compressor to be used when producing messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageCompressor"/> interface</typeparam>
        /// <returns></returns>
        IProducerConfigurationBuilder UseCompressor<T>()
            where T : IMessageCompressor;

        /// <summary>
        /// Set the default compressor to be used when producing messages
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageCompressor"/> interface</typeparam>
        /// <returns></returns>
        IProducerConfigurationBuilder UseCompressor<T>(Factory<T> factory)
            where T : IMessageCompressor;

        /// <summary>
        /// Set the default topic to be used when producing messages
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns></returns>
        IProducerConfigurationBuilder DefaultTopic(string topic);

        /// <summary>
        /// Set the <see cref="Confluent.Kafka.Acks"/> to be used when producing messages
        /// </summary>
        /// <param name="acks"></param>
        /// <returns></returns>
        // IProducerConfigurationBuilder WithAcks(Acks acks);
    }
}
