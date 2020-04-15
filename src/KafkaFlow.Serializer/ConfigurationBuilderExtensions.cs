namespace KafkaFlow.Serializer
{
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(this IConsumerMiddlewareConfigurationBuilder consumer)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            return consumer.AddSerializer(
                provider => provider.GetRequiredService<TSerializer>(),
                provider => provider.GetRequiredService<TResolver>());
        }

        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="middlewares"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            middlewares.ServiceCollection.TryAddSingleton<IMessageSerializer, TSerializer>();
            middlewares.ServiceCollection.TryAddSingleton<IMessageTypeResolver, TResolver>();
            middlewares.ServiceCollection.TryAddSingleton<TSerializer>();
            middlewares.ServiceCollection.TryAddSingleton<TResolver>();

            return middlewares.Add(
                provider => new SerializerConsumerMiddleware(
                    serializerFactory(provider),
                    resolverFactory(provider)));
        }

        /// <summary>
        /// Register a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(this IProducerMiddlewareConfigurationBuilder producer)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            return producer.AddSerializer(
                provider => provider.GetRequiredService<TSerializer>(),
                provider => provider.GetRequiredService<TResolver>());
        }

        /// <summary>
        /// Register a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="middlewares"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            middlewares.ServiceCollection.TryAddSingleton<IMessageSerializer, TSerializer>();
            middlewares.ServiceCollection.TryAddSingleton<IMessageTypeResolver, TResolver>();
            middlewares.ServiceCollection.TryAddSingleton<TSerializer>();
            middlewares.ServiceCollection.TryAddSingleton<TResolver>();

            return middlewares.Add(
                provider => new SerializerProducerMiddleware(
                    serializerFactory(provider),
                    resolverFactory(provider)));
        }
    }
}
