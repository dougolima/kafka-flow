namespace KafkaFlow.Extensions
{
    using System;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafka(
            this IServiceCollection services,
            Action<IKafkaConfigurationBuilder> kafka)
        {
            var builder = new KafkaConfigurationBuilder(services);

            kafka(builder);

            services.AddSingleton(builder.Build());

            return services;
        }
    }
}
