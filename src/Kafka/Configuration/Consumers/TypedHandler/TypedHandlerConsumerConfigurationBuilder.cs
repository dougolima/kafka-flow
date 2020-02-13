namespace Kafka.Configuration.Consumers.TypedHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kafka.Consumers;
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

        public TypedHandlerConsumerConfigurationBuilder UseSerializer<TSerializer>()
            where TSerializer : IMessageSerializer
        {
            this.serializer = typeof(TSerializer);
            return this;
        }

        public TypedHandlerConsumerConfigurationBuilder UseCompressor<TCompessor>()
            where TCompessor : IMessageCompressor
        {
            this.compressor = typeof(TCompessor);
            return this;
        }

        public TypedHandlerConsumerConfigurationBuilder WithNoCompressor()
        {
            this.compressor = typeof(NullMessageCompressor);
            return this;
        }

        public TypedHandlerConsumerConfigurationBuilder ScanHandlersFromAssemblyOf<T>()
            where T : IMessageHandler
        {
            var handlersType = typeof(T).Assembly
                .GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(x));

            this.handlers.AddRange(handlersType);

            return this;
        }

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
