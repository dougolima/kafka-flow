namespace KafkaFlow
{
    using System;
    using Confluent.Kafka;
    using KafkaFlow.Consumers;

    public class MessageContext
    {
        private readonly IOffsetManager offsetManager;
        private readonly ConsumeResult<byte[], byte[]> kafkaResult;

        public MessageContext(ConsumerMessage message,
            IOffsetManager offsetManager,
            int workerId,
            Type serializer,
            Type compressor)
        {
            this.offsetManager = offsetManager;
            this.Message = message;
            this.WorkerId = workerId;
            this.Serializer = serializer;
            this.Compressor = compressor;
            this.kafkaResult = message.KafkaResult;
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

        public int WorkerId { get; }

        public Type MessageType { get; set; }

        public object MessageObject { get; set; }

        public Type Serializer { get; set; }

        public Type Compressor { get; set; }

        public string Topic { get; set; }

        public Partition? Partition { get; set; }

        public Offset? Offset { get; set; }

        /// <summary>
        /// Store the message offset when manual store option is used
        /// </summary>
        public void StoreOffset()
        {
            if (this.offsetManager == null)
            {
                throw new InvalidOperationException("You can only store offsets in consumers");
            }

            this.offsetManager.StoreOffset(this.kafkaResult.TopicPartitionOffset);
        }
    }
}
