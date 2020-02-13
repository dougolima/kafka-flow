namespace Kafka
{
    using System;

    public class MessageMetadata
    {
        public Type MessageType { get; }

        public Type SerializerType { get; }

        public Type CompressorType { get; }

        public MessageMetadata(Type messageType, Type serializerType, Type compressorType)
        {
            if(!typeof(IMessageSerializer).IsAssignableFrom(serializerType))
            {
                throw new InvalidOperationException($"{serializerType.Name} should implement the interface {nameof(IMessageSerializer)}");
            }

            if(!typeof(IMessageCompressor).IsAssignableFrom(compressorType))
            {
                throw new InvalidOperationException($"{compressorType.Name} should implement the interface {nameof(IMessageCompressor)}");
            }

            this.MessageType = messageType;
            this.SerializerType = serializerType;
            this.CompressorType = compressorType;
        }
    }
}
