namespace KafkaFlow.Configuration
{
    using System;

    public class MiddlewareDefinition
    {
        public Type MiddlewareType { get; }
        public Action<IMessageMiddleware, IServiceProvider> Configurator { get; }

        public MiddlewareDefinition(Type middlewareType, Action<IMessageMiddleware, IServiceProvider> configurator)
        {
            this.MiddlewareType = middlewareType;
            this.Configurator = configurator;
        }
    }
}
