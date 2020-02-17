namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public class PartitionOffsets
    {
        private readonly LinkedList<long> pendingOffsets = new LinkedList<long>();

        public long LastOffset { get; private set; } = Offset.Unset;

        public void InitializeLastOffset(long offset)
        {
            if (this.LastOffset != Offset.Unset)
            {
                throw new InvalidOperationException("LastOffset is already initialized");
            }

            this.LastOffset = offset;
        }

        public bool ShouldUpdateOffset(long newOffset)
        {
            if (this.LastOffset == Offset.Unset)
            {
                throw new InvalidOperationException($"Call '{nameof(this.InitializeLastOffset)}()' first");
            }

            if (newOffset != this.LastOffset + 1)
            {
                this.pendingOffsets.AddLast(newOffset);
                return false;
            }

            do
            {
                ++this.LastOffset;
            } while (this.pendingOffsets.Remove(this.LastOffset + 1));

            return true;
        }
    }
}
