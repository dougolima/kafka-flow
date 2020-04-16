namespace KafkaFlow
{
    using System;

    internal struct ProducerMessageContext : IMessageContext
    {
        public ProducerMessageContext(
            object message,
            byte[] partitionKey,
            IMessageHeaders headers,
            string topic)
        {
            this.Message = message;
            this.MessageType = message.GetType();
            this.PartitionKey = partitionKey;
            this.Headers = headers ?? new MessageHeaders();
            this.Topic = topic;
            this.Offset = null;
            this.Partition = null;
        }

        public int WorkerId => 0;

        public byte[] PartitionKey { get; }

        public object Message { get; private set; }

        public Type MessageType { get; private set; }

        public IMessageHeaders Headers { get; }

        public string Topic { get; }

        public string GroupId => null;

        public int? Partition { get; set; }

        public long? Offset { get; set; }

        public void TransformMessage(object message, Type type)
        {
            this.Message = message;
            this.MessageType = type;
        }

        public IMessageConsumer Consumer => null;
    }
}