namespace Kafka
{
    using System;

    public class MessageContext
    {
        public MessageContext(IMessage message, Type serializer, Type compressor)
        {
            this.Message = message;
            this.Serializer = serializer;
            this.Compressor = compressor;
        }

        public IMessage Message { get; }

        public Type MessageType { get; set; }

        public Type Serializer { get; set; }

        public Type Compressor { get; set; }
    }
}
