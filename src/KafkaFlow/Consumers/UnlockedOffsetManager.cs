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
            var offsets = this.partitionsOffsets[offset.Partition.Value];

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
            var offsets = this.partitionsOffsets[message.KafkaResult.Partition.Value];

            if (offsets.LastOffset == Offset.Unset)
            {
                offsets.InitializeLastOffset(message.KafkaResult.Offset.Value - 1);
            }
        }
    }
}
