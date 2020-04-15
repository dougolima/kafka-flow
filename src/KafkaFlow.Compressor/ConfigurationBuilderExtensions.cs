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
        public static IConsumerMiddlewareConfigurationBuilder AddCompressor<T>(this IConsumerMiddlewareConfigurationBuilder middlewares)
            where T : class, IMessageCompressor
        {
            return middlewares.AddCompressor(provider => provider.GetRequiredService<T>());
        }

        /// <summary>
        /// Register a middleware to decompress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        /// <param name="middlewares"></param>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddCompressor<T>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<T> factory)
            where T : class, IMessageCompressor
        {
            middlewares.ServiceCollection.TryAddSingleton<IMessageCompressor, T>();
            middlewares.ServiceCollection.TryAddSingleton<T>();

            return middlewares.Add(
                provider => new CompressorConsumerMiddleware(factory(provider)));
        }

        /// <summary>
        /// Register a middleware to compress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(this IProducerMiddlewareConfigurationBuilder middlewares)
            where T : class, IMessageCompressor
        {
            return middlewares.AddCompressor(provider => provider.GetRequiredService<T>());
        }

        /// <summary>
        /// Register a middleware to compress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        /// <param name="middlewares"></param>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<T> factory)
            where T : class, IMessageCompressor
        {
            middlewares.ServiceCollection.TryAddSingleton<IMessageCompressor, T>();
            middlewares.ServiceCollection.TryAddSingleton<T>();

            return middlewares.Add(
                provider => new CompressorProducerMiddleware(factory(provider)));
        }
    }
}
