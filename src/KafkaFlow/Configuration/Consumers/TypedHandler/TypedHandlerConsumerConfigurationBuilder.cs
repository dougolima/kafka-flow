namespace KafkaFlow.Configuration.Consumers.TypedHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Consumers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class TypedHandlerConsumerConfigurationBuilder : ConsumerConfigurationBuilder<TypedHandlerConsumerConfigurationBuilder>
    {
        private Type serializer;
        private Type compressor;

        private readonly IServiceCollection services;
        private readonly List<Type> handlers = new List<Type>();

        public TypedHandlerConsumerConfigurationBuilder(IServiceCollection services)
            : base(services)
        {
            this.services = services;
        }

        /// <summary>
        /// Set the default serializer to be used when consuming messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements the <see cref="IMessageSerializer"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConsumerConfigurationBuilder UseSerializer<TSerializer>()
            where TSerializer : IMessageSerializer
        {
            this.serializer = typeof(TSerializer);
            return this;
        }

        /// <summary>
        /// Set the default compressor to be used when consuming messages
        /// </summary>
        /// <typeparam name="TCompessor">A class that implements the <see cref="IMessageCompressor"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConsumerConfigurationBuilder UseCompressor<TCompessor>()
            where TCompessor : IMessageCompressor
        {
            this.compressor = typeof(TCompessor);
            return this;
        }

        /// <summary>
        /// Configure the consumer to use no compression
        /// </summary>
        /// <returns></returns>
        public TypedHandlerConsumerConfigurationBuilder WithNoCompressor()
        {
            this.compressor = typeof(NullMessageCompressor);
            return this;
        }

        /// <summary>
        /// Register all classes that implements the <see cref="IMessageHandler{TMessage}"/> interface from the assembly of the provided type
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConsumerConfigurationBuilder ScanHandlersFromAssemblyOf<T>()
            where T : IMessageHandler
        {
            var handlersType = typeof(T).Assembly
                .GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(x));

            this.handlers.AddRange(handlersType);

            return this;
        }

        /// <summary>
        /// Manually informs the message handlers used by the consumer
        /// </summary>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public TypedHandlerConsumerConfigurationBuilder AddHandlers(IEnumerable<Type> handlers)
        {
            this.handlers.AddRange(handlers);
            return this;
        }

        public override ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var baseConfiguration = base.Build(clusterConfiguration);

            var configuration = new TypedHandlerConsumerConfiguration(
                baseConfiguration,
                this.serializer,
                this.compressor);

            this.services.TryAddSingleton(configuration.Serializer);
            this.services.TryAddSingleton(configuration.Compressor);

            foreach (var handlerType in this.handlers)
            {
                this.services.TryAddTransient(handlerType);

                var interfaceTypes = handlerType
                    .GetInterfaces()
                    .Where(x => x.IsGenericType && typeof(IMessageHandler).IsAssignableFrom(x));

                foreach (var interfaceType in interfaceTypes)
                {
                    configuration.HandlerMapping.AddMapping(interfaceType.GenericTypeArguments[0], handlerType);
                }
            }

            return configuration;
        }
    }
}
