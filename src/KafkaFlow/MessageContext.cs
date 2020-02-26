namespace KafkaFlow
{
    using System;
    using System.Text;
    using Confluent.Kafka;
    using KafkaFlow.Consumers;

    public class MessageContext : IMessageContext
    {
        private readonly IOffsetManager offsetManager;
        private readonly ConsumeResult<byte[], byte[]> kafkaResult;

        public MessageContext(
            ConsumerMessage message,
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
            this.Partition = message.KafkaResult.Partition.Value;
            this.Offset = message.KafkaResult.Offset.Value;
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

        public int? Partition { get; set; }

        public long? Offset { get; set; }

        public void StoreOffset()
        {
            if (this.offsetManager == null)
            {
                throw new InvalidOperationException("You can only store offsets in consumers");
            }

            this.offsetManager.StoreOffset(this.kafkaResult.TopicPartitionOffset);
        }

        public string GetStringHeader(string key, Encoding encoding)
        {
            return this.Message.Headers.TryGetValue(key, out var data) ?
                encoding.GetString(data) :
                null;
        }

        public string GetStringHeader(string key) => this.GetStringHeader(key, Encoding.UTF8);
    }
}
