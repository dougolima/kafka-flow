namespace KafkaFlow
{
    using System;

    public interface IMessageContext
    {
        int WorkerId { get; }

        byte[] RawMessage { get; }

        byte[] PartitionKey { get; }

        object Message { get; }

        Type MessageType { get; }

        IMessageHeaders Headers { get; }

        string Topic { get; }

        int? Partition { get; }

        long? Offset { get; }

        string GroupId { get; }

        void TransformMessage(object message, Type type);
        
        IMessageConsumer Consumer { get; }
    }
}
