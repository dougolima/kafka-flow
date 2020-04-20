namespace KafkaFlow
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Confluent.Kafka;

    internal class MessageHeaders : IMessageHeaders
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

        public byte[] this[string key]
        {
            get => this.headers.TryGetLastBytes(key, out var value) ? value : null;
            set
            {
                this.headers.Remove(key);
                this.headers.Add(key, value);
            }
        }

        public string GetString(string key, Encoding encoding)
        {
            return this.headers.TryGetLastBytes(key, out var value) ? encoding.GetString(value) : null;
        }

        public string GetString(string key) => this.GetString(key, Encoding.UTF8);

        public Headers GetKafkaHeaders() => this.headers;

        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
        {
            foreach (var header in this.headers)
            {
                yield return new KeyValuePair<string, byte[]>(header.Key, header.GetValueBytes());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
