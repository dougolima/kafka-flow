namespace KafkaFlow
{
    using System;
    using System.Text;

    public interface IMessageContext
    {
        IMessage Message { get; }

        int WorkerId { get; }

        Type MessageType { get; set; }

        object MessageObject { get; set; }

        IMessageSerializer Serializer { get; set; }

        IMessageCompressor Compressor { get; set; }

        string Topic { get; }

        int? Partition { get; }

        long? Offset { get; }

        /// <summary>
        /// Store the message offset when manual store option is used
        /// </summary>
        void StoreOffset();

        /// <summary>
        /// Get a header value as string
        /// </summary>
        /// <param name="key">The header key</param>
        /// <param name="encoding">The string format used to decode the value</param>
        /// <returns></returns>
        string GetStringHeader(string key, Encoding encoding);

        /// <summary>
        /// Get a header value as an UTF8 string
        /// </summary>
        /// <param name="key">The header key</param>
        /// <returns></returns>
        string GetStringHeader(string key);
    }
}
