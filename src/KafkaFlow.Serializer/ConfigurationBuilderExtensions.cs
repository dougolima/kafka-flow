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
        public static IConsumerConfigurationBuilder UseSerializerMiddleware<TSerializer, TResolver>(
            this IConsumerConfigurationBuilder consumer)
            where TSerializer : IMessageSerializer
            where TResolver : IMessageTypeResolver
        {
            return consumer.UseSerializerMiddleware(
                provider => provider.GetRequiredService<TSerializer>(),
                provider => provider.GetRequiredService<TResolver>());
        }

        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="consumer"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        public static IConsumerConfigurationBuilder UseSerializerMiddleware<TSerializer, TResolver>(
            this IConsumerConfigurationBuilder consumer,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : IMessageSerializer
            where TResolver : IMessageTypeResolver
        {
            consumer.ServiceCollection.TryAddSingleton(typeof(TSerializer));
            consumer.ServiceCollection.TryAddSingleton(typeof(TResolver));

            return consumer.UseMiddleware(provider => new SerializerConsumerMiddleware(
                serializerFactory(provider),
                resolverFactory(provider)));
        }

        /// <summary>
        /// Register a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        public static IProducerConfigurationBuilder UseSerializerMiddleware<TSerializer, TResolver>(
            this IProducerConfigurationBuilder consumer)
            where TSerializer : IMessageSerializer
            where TResolver : IMessageTypeResolver
        {
            return consumer.UseSerializerMiddleware(
                provider => provider.GetRequiredService<TSerializer>(),
                provider => provider.GetRequiredService<TResolver>());
        }

        /// <summary>
        /// Register a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="consumer"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        public static IProducerConfigurationBuilder UseSerializerMiddleware<TSerializer, TResolver>(
            this IProducerConfigurationBuilder consumer,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : IMessageSerializer
            where TResolver : IMessageTypeResolver
        {
            consumer.ServiceCollection.TryAddSingleton(typeof(TSerializer));
            consumer.ServiceCollection.TryAddSingleton(typeof(TResolver));

            return consumer.UseMiddleware(provider => new SerializerProducerMiddleware(
                serializerFactory(provider),
                resolverFactory(provider)));
        }
    }
}
