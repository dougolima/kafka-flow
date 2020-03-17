namespace KafkaFlow
{
    public interface IMessageContext
    {
        int WorkerId { get; }

        byte[] RawMessage { get; }

        byte[] PartitionKey { get; }

        object Message { get; }

        IMessageHeaders Headers { get; }

        string Topic { get; }

        int? Partition { get; }

        long? Offset { get; }

        string GroupId { get; }

        void TransformMessage(object message);

        /// <summary>
        /// Store the message offset when manual store option is used
        /// </summary>
        void StoreOffset();

        IOffsetsWatermark GetOffsetsWatermark();
    }
}
