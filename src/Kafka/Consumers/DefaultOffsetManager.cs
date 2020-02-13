namespace Kafka.Consumers
{
    using Confluent.Kafka;

    public class DefaultOffsetManager : IOffsetManager
    {
        private readonly IConsumer<byte[], byte[]> consumer;

        public DefaultOffsetManager(IConsumer<byte[], byte[]> consumer)
        {
            this.consumer = consumer;
        }

        public void StoreOffset(TopicPartitionOffset offset)
        {
            this.consumer.StoreOffset(
                new TopicPartitionOffset(
                    offset.TopicPartition,
                    new Offset(offset.Offset.Value + 1)));
        }
    }
}
