namespace KafkaFlow
{
    using Confluent.Kafka;
    using KafkaFlow.Consumers;

    internal readonly struct MessageConsumerWrapper : IMessageConsumer
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly IOffsetManager offsetManager;
        private readonly ConsumeResult<byte[], byte[]> kafkaResult;

        public MessageConsumerWrapper(
            IConsumer<byte[], byte[]> consumer,
            IOffsetManager offsetManager,
            ConsumeResult<byte[], byte[]> kafkaResult)
        {
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.kafkaResult = kafkaResult;
        }

        public void StoreOffset()
        {
            this.offsetManager.StoreOffset(this.kafkaResult.TopicPartitionOffset);
        }

        public IOffsetsWatermark GetOffsetsWatermark()
        {
            return new OffsetsWatermark(this.consumer.GetWatermarkOffsets(this.kafkaResult.TopicPartition));
        }

        public void Pause()
        {
            this.consumer.Pause(this.consumer.Assignment);
        }

        public void Resume()
        {
            this.consumer.Resume(this.consumer.Assignment);
        }
    }
}
