namespace KafkaFlow.TypedHandler
{
    using System;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ConsumerConfigurationBuilderExtensions
    {
        public static IConsumerConfigurationBuilder UseTypedHandlers(
            this IConsumerConfigurationBuilder consumer,
            Action<TypedHandlerConfigurationBuilder> configure)
        {
            var builder = new TypedHandlerConfigurationBuilder(consumer.ServiceCollection);

            configure(builder);

            var configuration = builder.Build();

            consumer.ServiceCollection.TryAddSingleton(configuration);
            consumer.UseMiddleware(provider => new TypedHandlerMiddleware(provider, configuration));

            return consumer;
        }
    }
}
