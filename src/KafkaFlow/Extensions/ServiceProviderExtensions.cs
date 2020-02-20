namespace KafkaFlow.Extensions
{
    using System;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceProviderExtensions
    {
        public static IKafkaBus UseKafka(this IServiceProvider provider)
        {
            var scope = provider.CreateScope();

            return new KafkaBus(
                scope.ServiceProvider.GetRequiredService<KafkaConfiguration>(),
                scope.ServiceProvider.GetRequiredService<ILogHandler>(),
                scope.ServiceProvider);
        }
    }
}
