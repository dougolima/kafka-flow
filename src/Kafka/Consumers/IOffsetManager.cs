namespace Kafka.Consumers
{
    using Confluent.Kafka;

    public interface IOffsetManager
    {
        void StoreOffset(TopicPartitionOffset offset);
    }
}