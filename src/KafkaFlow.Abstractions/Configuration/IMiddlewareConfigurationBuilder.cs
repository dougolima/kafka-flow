namespace KafkaFlow.Configuration
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IMiddlewareConfigurationBuilder
    {
        IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// Register a middleware
        /// </summary>
        /// <param name="factory">A factory to create the instance</param>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IMiddlewareConfigurationBuilder Add<T>(Factory<T> factory)
            where T : class, IMessageMiddleware;

        /// <summary>
        /// Register a middleware
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IMiddlewareConfigurationBuilder Add<T>()
            where T : class, IMessageMiddleware;
    }
}
