namespace KafkaFlow.Configuration.Consumers.TypedHandler
{
    using System;

    public class TypedHandlerConsumerConfiguration : ConsumerConfiguration
    {
        public Type Serializer { get; }

        public Type Compressor { get; }

        public HandlerTypeMapping HandlerMapping { get; } = new HandlerTypeMapping();

        public TypedHandlerConsumerConfiguration(
            ConsumerConfiguration baseConfiguration,
            Type serializer,
            Type compressor)
            : base(baseConfiguration)
        {
            this.Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.Compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
        }
    }
}
