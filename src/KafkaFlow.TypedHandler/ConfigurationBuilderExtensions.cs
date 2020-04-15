namespace KafkaFlow.TypedHandler
{
    using System;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ConfigurationBuilderExtensions
    {
        public static IConsumerMiddlewareConfigurationBuilder AddTypedHandlers(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Action<TypedHandlerConfigurationBuilder> configure)
        {
            var builder = new TypedHandlerConfigurationBuilder(middlewares.ServiceCollection);

            configure(builder);

            var configuration = builder.Build();

            middlewares.ServiceCollection.TryAddSingleton(configuration);
            middlewares.Add(provider => new TypedHandlerMiddleware(provider, configuration));

            return middlewares;
        }
    }
}
