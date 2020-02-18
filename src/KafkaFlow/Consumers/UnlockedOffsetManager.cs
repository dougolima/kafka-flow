namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;

    public class UnlockedOffsetManager : IOffsetManager
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly Dictionary<int, PartitionOffsets> partitionsOffsets;

        public UnlockedOffsetManager(
            IConsumer<byte[], byte[]> consumer,
            IEnumerable<TopicPartition> partitions)
        {
            this.consumer = consumer;

            this.partitionsOffsets = partitions.ToDictionary(
                partition => partition.Partition.Value,
                partition => new PartitionOffsets());
        }

        public void StoreOffset(TopicPartitionOffset offset)
        {
            if (!this.partitionsOffsets.TryGetValue(offset.Partition.Value, out var offsets))
            {
                return;
            }

            lock (offsets)
            {
                if (offsets.ShouldUpdateOffset(offset.Offset.Value))
                {
                    this.consumer.StoreOffset(
                        new TopicPartitionOffset(
                            offset.TopicPartition,
                            new Offset(offsets.LastOffset + 1)));
                }
            }
        }

        public void InitializeOffsetIfNeeded(ConsumerMessage message)
        {
            if (
                this.partitionsOffsets.TryGetValue(message.KafkaResult.Partition.Value, out var offsets) &&
                offsets.LastOffset == Offset.Unset)
            {
                offsets.InitializeLastOffset(message.KafkaResult.Offset.Value - 1);
            }
        }
    }
}
