namespace KafkaFlow.Extensions
{
    using System;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// </summary>
    public static class ServiceProviderExtensions
    {
        public static IKafkaBus UseKafka(this IServiceProvider provider)
        {
            var scope = provider.CreateScope();

            return new KafkaBus(
                scope.ServiceProvider.GetRequiredService<KafkaConfiguration>(),
                ConsumerManager.Instance,
                scope.ServiceProvider.GetRequiredService<ILogHandler>(),
                scope.ServiceProvider);
        }
    }
}
