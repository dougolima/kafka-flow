namespace KafkaFlow
{
    using System;
    using Confluent.Kafka;
    using KafkaFlow.Consumers;

    internal class MessageContext : IMessageContext
    {
        private readonly IOffsetManager offsetManager;

        public MessageContext(
            ConsumeResult<byte[], byte[]> kafkaResult,
            IOffsetManager offsetManager,
            int workerId,
            string groupId)
        {
            this.KafkaResult = kafkaResult;
            this.offsetManager = offsetManager;
            this.Message = this.RawMessage = kafkaResult.Value;
            this.PartitionKey = kafkaResult.Key;
            this.Headers = new MessageHeaders(kafkaResult.Headers);
            this.WorkerId = workerId;
            this.Topic = kafkaResult.Topic;
            this.Partition = kafkaResult.Partition.Value;
            this.Offset = kafkaResult.Offset.Value;
            this.GroupId = groupId;
        }

        public MessageContext(
            object message,
            byte[] partitionKey,
            IMessageHeaders headers,
            string topic)
        {
            this.Message = message;
            this.PartitionKey = partitionKey;
            this.Headers = headers ?? new MessageHeaders();
            this.Topic = topic;
        }

        public ConsumeResult<byte[], byte[]> KafkaResult { get; }

        public int WorkerId { get; }

        public byte[] RawMessage { get; }

        public byte[] PartitionKey { get; }

        public object Message { get; private set; }

        public Type MessageType { get; private set; }

        public IMessageHeaders Headers { get; }

        public string Topic { get; }

        public string GroupId { get; }

        public int? Partition { get; set; }

        public long? Offset { get; set; }

        public void TransformMessage(object message, Type type)
        {
            this.Message = message;
            this.MessageType = type;
        }

        public void StoreOffset()
        {
            if (this.offsetManager == null)
            {
                throw new InvalidOperationException("You can only store offsets in consumers");
            }

            this.offsetManager.StoreOffset(this.KafkaResult.TopicPartitionOffset);
        }

        public IOffsetsWatermark GetOffsetsWatermark()
        {
            return this.offsetManager.GetOffsetWatermark(this.KafkaResult.TopicPartition);
        }
    }
}
