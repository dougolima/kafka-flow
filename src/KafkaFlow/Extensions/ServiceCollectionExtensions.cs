namespace KafkaFlow.Extensions
{
    using System;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
    }
}
