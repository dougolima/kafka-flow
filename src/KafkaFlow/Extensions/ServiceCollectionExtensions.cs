namespace KafkaFlow.Extensions
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafka(
            this IServiceCollection services,
            Action<KafkaConfigurationBuilder> kafka)
        {
            var builder = new KafkaConfigurationBuilder(services);

            kafka(builder);

            services.AddSingleton(builder.Build());

            return services;
        }

        internal static IServiceCollection AddMiddlewares(this IServiceCollection services, IEnumerable<ConfigurableDefinition<IMessageMiddleware>> middlewares)
        {
            foreach (var middleware in middlewares)
            {
                services.TryAddSingleton(middleware.Type);
            }

            return services;
        }
    }
}
