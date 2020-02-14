namespace KafkaFlow.Consumers
{
    using Confluent.Kafka;

    public interface IOffsetManager
    {
        void StoreOffset(TopicPartitionOffset offset);
    }
}