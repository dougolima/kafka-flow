namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class ConsumerMiddlewareConfigurationBuilder
        : IConsumerMiddlewareConfigurationBuilder
    {
        public IServiceCollection ServiceCollection { get; }

        private readonly List<Factory<IMessageMiddleware>> middlewaresFactories = new List<Factory<IMessageMiddleware>>();

        public ConsumerMiddlewareConfigurationBuilder(IServiceCollection serviceCollection)
        {
            this.ServiceCollection = serviceCollection;
        }

        public IConsumerMiddlewareConfigurationBuilder Add<T>(Factory<T> factory) where T : class, IMessageMiddleware
        {
            this.ServiceCollection.TryAddScoped<IMessageMiddleware, T>();
            this.ServiceCollection.TryAddScoped<T>();
            this.middlewaresFactories.Add(factory);
            return this;
        }

        public IConsumerMiddlewareConfigurationBuilder Add<T>() where T : class, IMessageMiddleware
        {
            return this.Add(provider => provider.GetRequiredService<T>());
        }

        public MiddlewareConfiguration Build() => new MiddlewareConfiguration(this.middlewaresFactories);
    }
}