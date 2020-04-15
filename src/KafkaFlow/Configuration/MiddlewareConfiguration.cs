namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    public class MiddlewareConfiguration
    {
        public IReadOnlyCollection<Factory<IMessageMiddleware>> Factories { get; }

        public MiddlewareConfiguration(IReadOnlyCollection<Factory<IMessageMiddleware>> factories)
        {
            this.Factories = factories;
        }
    }
}
