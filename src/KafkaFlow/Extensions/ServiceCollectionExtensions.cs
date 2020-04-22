namespace KafkaFlow.Extensions
{
    using System;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
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

            services.AddSingleton<IConsumerAccessor>(ConsumerManager.Instance);

            return services;
        }
    }
}
