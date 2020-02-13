namespace Kafka.Configuration
{
    using System;

    public class MiddlewareDefinition
    {
        public Type MiddlewareType { get; }
        public Action<IMessageMiddleware> Configurator { get; }

        public MiddlewareDefinition(Type middlewareType, Action<IMessageMiddleware> configurator)
        {
            this.MiddlewareType = middlewareType;
            this.Configurator = configurator;
        }
    }
}
