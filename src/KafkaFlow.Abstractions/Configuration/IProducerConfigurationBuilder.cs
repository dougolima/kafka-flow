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
        /// Set the default topic to be used when producing messages
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns></returns>
        IProducerConfigurationBuilder DefaultTopic(string topic);

        /// <summary>
        /// Set the <see cref="Acks"/> to be used when producing messages
        /// </summary>
        /// <param name="acks"></param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithAcks(Acks acks);
    }
}
