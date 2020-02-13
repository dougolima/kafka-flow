namespace Kafka
{
    using System.Collections.Generic;

    public class ProducerMessage : IMessage
    {
        public ProducerMessage(byte[] key, byte[] value, Dictionary<string, byte[]> headers)
        {
            this.Key = key;
            this.Value = value;
            this.Headers = headers ?? new Dictionary<string, byte[]>();
        }

        public byte[] Key { get; }

        public byte[] Value { get; }

        public Dictionary<string, byte[]> Headers { get; }
    }
}
