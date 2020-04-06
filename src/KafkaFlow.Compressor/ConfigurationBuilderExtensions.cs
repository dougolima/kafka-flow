namespace KafkaFlow.Compressor
{
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Register a middleware to decompress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        public static IConsumerConfigurationBuilder UseCompressorMiddleware<T>(this IConsumerConfigurationBuilder consumer)
            where T : class, IMessageCompressor
        {
            return consumer.UseCompressorMiddleware(provider => provider.GetRequiredService<T>());
        }

        /// <summary>
        /// Register a middleware to decompress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        /// <param name="consumer"></param>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder UseCompressorMiddleware<T>(
            this IConsumerConfigurationBuilder consumer,
            Factory<T> factory)
            where T : class, IMessageCompressor
        {
            consumer.ServiceCollection.TryAddSingleton<IMessageCompressor, T>();
            consumer.ServiceCollection.TryAddSingleton<T>();

            return consumer.UseMiddleware(provider => new CompressorConsumerMiddleware(factory(provider)));
        }

        /// <summary>
        /// Register a middleware to compress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        public static IProducerConfigurationBuilder UseCompressorMiddleware<T>(this IProducerConfigurationBuilder producer)
            where T : class, IMessageCompressor
        {
            return producer.UseCompressorMiddleware(provider => provider.GetRequiredService<T>());
        }

        /// <summary>
        /// Register a middleware to compress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        /// <param name="producer"></param>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        public static IProducerConfigurationBuilder UseCompressorMiddleware<T>(
            this IProducerConfigurationBuilder producer,
            Factory<T> factory)
            where T : class, IMessageCompressor
        {
            producer.ServiceCollection.TryAddSingleton<IMessageCompressor, T>();
            producer.ServiceCollection.TryAddSingleton<T>();

            return producer.UseMiddleware(provider => new CompressorProducerMiddleware(factory(provider)));
        }
    }
}
