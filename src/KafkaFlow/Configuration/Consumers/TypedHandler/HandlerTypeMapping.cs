namespace KafkaFlow.Configuration.Consumers.TypedHandler
{
    using System;
    using System.Collections.Generic;

    public class HandlerTypeMapping
    {
        private readonly Dictionary<Type, Type> mapping = new Dictionary<Type, Type>();

        public void AddMapping(Type messageType, Type handlerType)
        {
            this.mapping.Add(messageType, handlerType);
        }

        public Type GetHandlerType(Type messageType)
        {
            return this.mapping.TryGetValue(messageType, out var handlerType) ? handlerType : null;
        }

        public IEnumerable<Type> Handlers => this.mapping.Values;
    }
}
