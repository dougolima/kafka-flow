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
            int workerId)
        {
            this.offsetManager = offsetManager;
            this.Message = this.RawMessage = kafkaResult.Value;
            this.Headers = new MessageHeaders(kafkaResult.Headers);
            this.KafkaResult = kafkaResult;
            this.WorkerId = workerId;
            this.Topic = kafkaResult.Topic;
            this.Partition = kafkaResult.Partition.Value;
            this.Offset = kafkaResult.Offset.Value;
        }

        public MessageContext(
            object message,
            IMessageHeaders headers,
            string topic)
        {
            this.Message = message;
            this.Headers = headers ?? new MessageHeaders();
            this.Topic = topic;
        }

        public ConsumeResult<byte[], byte[]> KafkaResult { get; }
        public int WorkerId { get; }

        public byte[] RawMessage { get; }

        public object Message { get; private set; }

        public IMessageHeaders Headers { get; }

        public string Topic { get; }

        public int? Partition { get; set; }

        public long? Offset { get; set; }

        public void TransformMessage(object message)
        {
            this.Message = message;
        }

        public void StoreOffset()
        {
            if (this.offsetManager == null)
            {
                throw new InvalidOperationException("You can only store offsets in consumers");
            }

            this.offsetManager.StoreOffset(this.KafkaResult.TopicPartitionOffset);
        }
    }
}
