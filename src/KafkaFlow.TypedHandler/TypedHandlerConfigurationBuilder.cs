namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class TypedHandlerConfigurationBuilder
    {
        private readonly IServiceCollection services;
        private readonly List<Type> handlers = new List<Type>();

        private ServiceLifetime serviceLifetime = ServiceLifetime.Singleton;

        public TypedHandlerConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// Register all classes that implements the <see cref="IMessageHandler{TMessage}"/> interface from the assembly of the provided type
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder ScanHandlersFromAssemblyOf<T>()
            where T : IMessageHandler
        {
            var handlersType = typeof(T).Assembly
                .GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(x));

            this.handlers.AddRange(handlersType);

            return this;
        }

        /// <summary>
        /// Manually adds the message handlers
        /// </summary>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandlers(IEnumerable<Type> handlers)
        {
            this.handlers.AddRange(handlers);
            return this;
        }

        /// <summary>
        /// Manually adds the message handler
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandler<T>()
            where T : IMessageHandler
        {
            this.handlers.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Set the handler lifetime. The default value is <see cref="ServiceLifetime.Singleton"/>
        /// </summary>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder WithHandlerLifetime(ServiceLifetime lifetime)
        {
            this.serviceLifetime = lifetime;
            return this;
        }

        public TypedHandlerConfiguration Build()
        {
            var configuration = new TypedHandlerConfiguration();

            foreach (var handlerType in this.handlers)
            {
                this.services.TryAdd(ServiceDescriptor.Describe(
                    handlerType,
                    handlerType,
                    this.serviceLifetime));

                var messageTypes = handlerType
                    .GetInterfaces()
                    .Where(x => x.IsGenericType && typeof(IMessageHandler).IsAssignableFrom(x))
                    .Select(x => x.GenericTypeArguments[0]);

                foreach (var messageType in messageTypes)
                {
                    configuration.HandlerMapping.AddMapping(messageType, handlerType);
                }
            }

            return configuration;
        }
    }
}
