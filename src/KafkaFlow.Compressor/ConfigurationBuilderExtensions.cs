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
        public static IConsumerConfigurationBuilder UseCompressorMiddleware<T>(
            this IConsumerConfigurationBuilder consumer)
            where T : IMessageCompressor
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
        public static IConsumerConfigurationBuilder UseCompressorMiddleware<T>(this IConsumerConfigurationBuilder consumer, Factory<T> factory)
            where T : IMessageCompressor
        {
            consumer.ServiceCollection.TryAddSingleton(typeof(T));

            return consumer.UseMiddleware(provider => new CompressorConsumerMiddleware(factory(provider)));
        }

        /// <summary>
        /// Register a middleware to compress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        public static IProducerConfigurationBuilder UseCompressorMiddleware<T>(
            this IProducerConfigurationBuilder consumer)
            where T : IMessageCompressor
        {
            return consumer.UseCompressorMiddleware(provider => provider.GetRequiredService<T>());
        }

        /// <summary>
        /// Register a middleware to compress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        /// <param name="consumer"></param>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        public static IProducerConfigurationBuilder UseCompressorMiddleware<T>(this IProducerConfigurationBuilder consumer, Factory<T> factory)
            where T : IMessageCompressor
        {
            consumer.ServiceCollection.TryAddSingleton(typeof(T));

            return consumer.UseMiddleware(provider => new CompressorProducerMiddleware(factory(provider)));
        }
    }
}
