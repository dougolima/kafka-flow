namespace KafkaFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;

    public class ConsumerMessage : IMessage
    {
        public ConsumerMessage(ConsumeResult<byte[], byte[]> consumeResult)
        {
            this.KafkaResult = consumeResult;
            this.Headers = consumeResult.Headers.ToDictionary(
                x => x.Key,
                x => x.GetValueBytes());
        }

        public byte[] Key => this.KafkaResult.Key;

        public byte[] Value => this.KafkaResult.Value;

        public Dictionary<string, byte[]> Headers { get; }

        public ConsumeResult<byte[], byte[]> KafkaResult { get; }
    }
}
