namespace Kafka
{
    using System;
    using Confluent.Kafka;

    public class MessageContext
    {
        public MessageContext(
            ConsumerMessage message,
            Type serializer,
            Type compressor)
        {
            this.Message = message;
            this.Serializer = serializer;
            this.Compressor = compressor;
            this.Topic = message.KafkaResult.Topic;
            this.Partition = message.KafkaResult.Partition;
            this.Offset = message.KafkaResult.Offset;
        }

        public MessageContext(
            IMessage message,
            Type serializer,
            Type compressor,
            string topic)
        {
            this.Message = message;
            this.Serializer = serializer;
            this.Compressor = compressor;
            this.Topic = topic;
        }

        public IMessage Message { get; }

        public Type MessageType { get; set; }

        public Type Serializer { get; set; }

        public Type Compressor { get; set; }

        public string Topic { get; set; }

        public Partition? Partition { get; set; }

        public Offset? Offset { get; set; }
    }
}
