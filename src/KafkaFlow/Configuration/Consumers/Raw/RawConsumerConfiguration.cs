namespace KafkaFlow.Configuration.Consumers.Raw
{
    using System;

    public class RawConsumerConfiguration : ConsumerConfiguration
    {
        public Type HandlerType { get; }

        public RawConsumerConfiguration(
            ConsumerConfiguration baseConfiguration,
            Type handlerType)
            : base(baseConfiguration)
        {
            this.HandlerType = handlerType;
        }
    }
}
