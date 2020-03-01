namespace KafkaFlow
{
    using System.Text;

    public interface IMessageHeaders
    {
        void Add(string key, byte[] value);

        byte[] this[string key] { get; }

        /// <summary>
        /// Get a header value as string
        /// </summary>
        /// <param name="key">The header key</param>
        /// <param name="encoding">The string format used to decode the value</param>
        /// <returns></returns>
        string GetString(string key, Encoding encoding);

        /// <summary>
        /// Get a header value as an UTF8 string
        /// </summary>
        /// <param name="key">The header key</param>
        /// <returns></returns>
        string GetString(string key);
    }
}
