namespace Kafka.Extensions
{
    using System;
    using Kafka.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceProviderExtensions
    {
        public static IKafkaBus UseKafka(this IServiceProvider provider)
        {
            return new KafkaBus(
                provider.GetRequiredService<KafkaConfiguration>(),
                provider.GetRequiredService<ILogHandler>(),
                provider);
        }
    }
}
