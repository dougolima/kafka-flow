namespace KafkaFlow
{
    using System.Text;
    using Confluent.Kafka;

    public class MessageHeaders : IMessageHeaders
    {
        private readonly Headers headers;

        public MessageHeaders(Headers headers)
        {
            this.headers = headers;
        }

        public MessageHeaders()
            : this(new Headers())
        {
        }

        public void Add(string key, byte[] value)
        {
            this.headers.Add(key, value);
        }

        public byte[] this[string key] => this.headers.TryGetLastBytes(key, out var value) ? value : null;

        public string GetString(string key, Encoding encoding)
        {
            return this.headers.TryGetLastBytes(key, out var value) ? encoding.GetString(value) : null;
        }

        public string GetString(string key) => this.GetString(key, Encoding.UTF8);

        public Headers GetKafkaHeaders() => this.headers;
    }
}
